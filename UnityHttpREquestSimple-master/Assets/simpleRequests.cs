using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;

public class simpleRequests : MonoBehaviour
{
    [SerializeField]
    TMP_Text texto;
    [SerializeField]
    List<NAbility> nAbility;
    private string pokeImage;
    private string pokeShiny;
    public RawImage Image1;
    public RawImage Image2;
    public TMP_InputField pokemon;
    public TMP_Text pokeName;
    public TMP_Text pokeNumber;
    public TMP_Text pokeType;
    public TMP_Text pokeType2;

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
                    texto.text = webRequest.downloadHandler.text;
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    JSONNode root = JSONNode.Parse(webRequest.downloadHandler.text);

                    foreach (var sprites in root["sprites"])
                    {
                        if(sprites.Value["home"] != null)
                        {
                            pokeImage = sprites.Value["home"]["front_default"];
                            pokeShiny = sprites.Value["home"]["front_shiny"];
                        }
                        
                    }

                    foreach (var species in root["forms"])
                    {
                        if (species.Value["name"] != null)
                        {
                            pokeName.text = "Name: " + species.Value["name"];
                        }

                    }

                    foreach (var number in root["game_indices"])
                    {
                        if (number.Value != null)
                        {
                            pokeNumber.text = "Number: " + number.Value["game_index"];
                        }

                    }

                    foreach (var types in root["types"])
                    {
                        if (types.Value["slot"].ToString() == "1")
                        {
                            pokeType.text = "Type: " + types.Value["type"]["name"];
                            
                        }
                        if (types.Value["slot"].ToString() == "2")
                        {
                            pokeType2.text = "and " + types.Value["type"]["name"];
                        }
                        else
                        {
                            pokeType2.text = "";
                        }

                    }

                    break;
            }
            
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(pokeImage);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                //Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Image1.texture = myTexture;
            }

            UnityWebRequest www2 = UnityWebRequestTexture.GetTexture(pokeShiny);
            yield return www2.SendWebRequest();

            if (www2.result != UnityWebRequest.Result.Success)
            {
                //Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www2.downloadHandler).texture;
                Image2.texture = myTexture;
            }

        }
    }

    public void ButtonSearch()
    {  
        if (pokemon != null)
        {
            StartCoroutine(GetRequest("https://pokeapi.co/api/v2/pokemon/" + pokemon.text.ToString().ToLower()));
        }
    }
}
