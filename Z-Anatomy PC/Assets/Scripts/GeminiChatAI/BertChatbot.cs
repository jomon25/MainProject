using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

public class FlaskChatManager : MonoBehaviour
{
    public TMP_InputField playerInputField;
    public Button submitButton; // Reference to the SubmitButton
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject newTextPrefab;
    public string flaskApiUrl = "http://127.0.0.1:5000/api/predict";

    void Start()
    {
        StartChat();
        submitButton.onClick.AddListener(OnSendButtonClick); // Add listener to the button
    }

    public void StartChat()
    {
        AddToChat("Bot: Hi! I'm ChatBot. How can I help you?");
    }

    public void OnSendButtonClick()
    {
        string playerMessage = playerInputField.text;
        if (string.IsNullOrEmpty(playerMessage)) return;

        AddPlayerChat($"Player: {playerMessage}");
        StartCoroutine(SendToFlask(playerMessage));
        playerInputField.text = "";
    }

    private void AddToChat(string message)
    {
        Debug.Log("AddToChat called, newTextPrefab: " + (newTextPrefab == null ? "null" : "assigned") + ", content: " + (content == null ? "null" : "assigned"));
        if (newTextPrefab == null || content == null)
        {
            Debug.LogError("AddToChat: newTextPrefab or content is null!");
            return;
        }
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<GeminiTypewritter>().StartTyping(message);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    private void AddPlayerChat(string message)
    {
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<TMP_Text>().text = message;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    private IEnumerator SendToFlask(string prompt)
    {
        string jsonPayload = JsonConvert.SerializeObject(new { question = prompt });

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(flaskApiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                Debug.Log(response);

                try
                {
                    Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                    if (responseData != null && responseData.ContainsKey("Answer"))
                    {
                        string answer = responseData["Answer"];
                        AddToChat($"Bot: {answer}");
                    }
                    else
                    {
                        AddToChat("Bot: Sorry, I didn't understand that.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON Parsing Error: " + e.Message);
                    AddToChat("Bot: Sorry, something went wrong.");
                }
            }
            else
            {
                Debug.LogError("Flask API Error: " + request.error);
                AddToChat("Bot: Sorry, something went wrong.");
            }
        }
    }
}