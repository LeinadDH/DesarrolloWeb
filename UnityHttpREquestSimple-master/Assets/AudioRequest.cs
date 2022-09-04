using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AudioRequest : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    void Start()
    {
        StartCoroutine(GetAudioClip());
    }

    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://www.televisiontunes.com/uploads/audio/Pokemon.mp3", AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip = myClip;
                audioSource.clip = audioClip;
                audioSource.Play();
                audioSource.loop = true;
            }
        }
    }
}
