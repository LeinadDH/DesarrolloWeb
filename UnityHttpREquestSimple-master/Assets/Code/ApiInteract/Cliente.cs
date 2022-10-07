using SimpleJSON;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Cliente : MonoBehaviour
{

    [Header ("Show students")]
    public TMP_Text nametxt;
    public TMP_Text studentIdtxt;
    public TMP_Text gpatxt;
    public TMP_Text careertxt;
    public TMP_Text emailtxt;
    public Toggle enrolledbool;
    public Image toggleImage;
    public Slider bar;

    [Header("Add info")]
    public TMP_InputField _name;
    public TMP_InputField _studentId;
    public TMP_InputField _gpa;
    public TMP_InputField _career;
    public TMP_InputField _email;
    public Toggle _isEnrolled;


    private string myApi = "http://localhost:7257/api/mymodels";
    
    public class MyModelTask
    {
        public int id;

        public string name = "";

        public int studentId = 0;

        public float gpa = 0;

        public string career = "";

        public string email = "";

        public bool isEnrolled;
    }

    public void Put()
    {
        StartCoroutine(PutRequest());
    }

    public void Post()
    {
        StartCoroutine(PostRequest());
    }

    public void Get()
    {
        StartCoroutine(GetRequest(myApi));
    }

    public void Delete()
    {
        StartCoroutine(DeleteRequest(myApi));
    }

    public void ERequest()
    {
        StartCoroutine(EspecificRequest(myApi));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }

            webRequest.Dispose();
        }
    }


    public IEnumerator PostRequest()
    {
        string json = JsonUtility.ToJson(JsonTask());

        var req = new UnityWebRequest(myApi, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Received: " + req.downloadHandler.text);        
        }
        else
        {
            Debug.Log("Error While Sending: " + req.error);
        }

        req.Dispose();

    }

    public IEnumerator DeleteRequest(String endpoint)
    {
        float id = bar.value;
        UnityWebRequest www = UnityWebRequest.Delete(endpoint + "/" + id);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.responseCode);
            Debug.Log(www.error);
        }
        Debug.Log("Delete succes");
        www.Dispose();
    }


    public IEnumerator PutRequest()
    {
        string json = JsonUtility.ToJson(JsonTask());

        UnityWebRequest www = UnityWebRequest.Put(myApi + "/" + JsonTask().id, json);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.responseCode);
            Debug.Log(www.error);
        }
        Debug.Log("Put succes");
        www.Dispose();
    }

    public MyModelTask JsonTask()
    {
        MyModelTask task = new MyModelTask();
        task.id = int.Parse(bar.value.ToString());
        task.name = _name.text.ToString();
        task.studentId = int.Parse(_studentId.text);
        task.gpa = float.Parse(_gpa.text);
        task.career = _career.text.ToString();
        task.email = _email.text.ToString();
        task.isEnrolled = _isEnrolled.isOn;

        return task;
    }

    IEnumerator EspecificRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    JSONNode root = JSONNode.Parse(webRequest.downloadHandler.text);

                    foreach (var sprites in root)
                    {
                        if (sprites.Value["id"] == bar.value)
                        {
                            _name.text = sprites.Value["name"];
                            _studentId.text = sprites.Value["studentID"];
                            _gpa.text = sprites.Value["gpa"];
                            _career.text = sprites.Value["career"];
                            _email.text = sprites.Value["email"];
                            _isEnrolled.isOn = sprites.Value["isEnrolled"];
                            if (sprites.Value["isEnrolled"] == true)
                            {
                                toggleImage.color = Color.green;
                            }
                            if (sprites.Value["isEnrolled"] == false)
                            {
                                toggleImage.color = Color.red;
                            }
                        }
                    }

                    break;
            }

        }
    }
}
