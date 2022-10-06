using SimpleJSON;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class Cliente : MonoBehaviour
{
    public int _id = 0;
    public string _name;
    public int _studentId;
    public float _gpa;
    public string _career;
    public string _email;
    public bool _isEnrolled;
    public TMP_Text texto;

    public string KeyRequest;

    private string myApi = "http://localhost:7257/api/mymodels";
    
    public class MyModelTask
    {
        public int id;

        public string name;

        public int studentId;

        public float gpa;

        public string career;

        public string email;

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
        int id = _id;
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
        task.id = _id;
        task.name = _name;
        task.studentId = _studentId;
        task.gpa = _gpa;
        task.career = _career;
        task.email = _email;
        task.isEnrolled = _isEnrolled;

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
                    texto.text = webRequest.downloadHandler.text;
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    JSONNode root = JSONNode.Parse(webRequest.downloadHandler.text);

                    foreach (var sprites in root)
                    {
                        if (sprites.Value["id"] == _id)
                        {
                            texto.text = sprites.Value[KeyRequest];
                        }
                    }

                    break;
            }

        }
    }
}
