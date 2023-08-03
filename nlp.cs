using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GameController4 : MonoBehaviour
{
    public GameObject player;  // The object that will be moved
    public float speed = 20f;  // Movement speed of the object
    public TMP_InputField playerInputField;
    public Button submitButton;
    private DictationRecognizer dictationRecognizer;

    // ArrayList to hold valid commands
    private ArrayList validCommands = new ArrayList { "up", "down", "left", "right", "red", "blue", "yellow", "white","black"  };

    void Start()
    {
        submitButton.onClick.AddListener(SubmitInput);

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            ProcessCommand(text);
        };
        dictationRecognizer.Start();
    }

    public void SubmitInput()
    {
        ProcessCommand(playerInputField.text);
        playerInputField.text = "";  // Clear input field after processing command
    }

    private void ProcessCommand(string command)
    {
        string phrase = command.ToLower();
        if (phrase.Contains("up"))
        {
            player.transform.Translate(0, speed * Time.deltaTime, 0);
        }
        else if (phrase.Contains("down"))
        {
            player.transform.Translate(0, -speed * Time.deltaTime, 0);
        }
        else if (phrase.Contains("left"))
        {
            player.transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (phrase.Contains("right"))
        {
            player.transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        else if (phrase.Contains("red"))
        {
            player.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (phrase.Contains("black"))
        {
            player.GetComponent<Renderer>().material.color = Color.black;
        }
        else if (phrase.Contains("blue"))
        {
            player.GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (phrase.Contains("white"))
        {
            player.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            Debug.Log("Invalid command: " + command);
        }
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
        }
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Dispose();
        }
    }
}

 private IEnumerator AnalyzeText(string text)
    {
        string apiKey = "Your-API-Key";
        string url = "https://language.googleapis.com/v1beta2/documents:analyzeSentiment?key=" + apiKey;
        var httpRequest = new UnityWebRequest(url, "POST");
        httpRequest.SetRequestHeader("Content-Type", "application/json");
        var jsonData = new RequestJsonData
        {
            document = new Document
            {
                type = "PLAIN_TEXT",
                content = text
            },
            encodingType = "UTF8"
        };

        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonData));
        httpRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        httpRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return httpRequest.SendWebRequest();

        if (httpRequest.result == UnityWebRequest.Result.Success)
        {
            var N = JSON.Parse(httpRequest.downloadHandler.text);
            var action = N["documentSentiment"]["magnitude"].AsFloat >= 0.25f ? "Attack" : "Run";
            npc.ProcessCommand(action);
            npcResponseText.text = npc.response;
        }
        else
        {
            Debug.Log("Error: " + httpRequest.error);
        }

        httpRequest.Dispose();  // Dispose the UnityWebRequest object
    }

  
[System.Serializable]
public class RequestJsonData
{
    public Document document;
    public string encodingType;
}

[System.Serializable]
public class Document
{
    public string type;
    public string content;
}
