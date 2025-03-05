using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using TMPro;

public class GeminiChatManager : MonoBehaviour
{
    public TMP_InputField playerInputField; // Player input field
    public string apiKey = "AIzaSyCcSNH_TOKt_OJQSF2kqp0x4YcuhGDZHAw"; // Replace with your actual API key
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=";

    public ScrollRect scrollRect; // Reference to ScrollRect component
    public RectTransform content; // Reference to Content GameObject
    public GameObject newTextPrefab; // Prefab for new text display

    void Start()
    {
        StartChat();
    }

    public void StartChat()
    {
        AddToChat("Bot: Hi! I'm ChatBot. How can I help you?");
    }

    public void OnSendButtonClick()
    {
        string playerMessage = playerInputField.text;
        if (string.IsNullOrEmpty(playerMessage)) return;

        AddPlayerChat($"Player: {playerMessage}"); // Display player message
        StartCoroutine(SendToGemini(playerMessage)); // Send to Gemini API
        playerInputField.text = ""; // Clear input field
    }

    private void AddToChat(string message)
    {
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<GeminiTypewritter>().StartTyping(message); // Updated class reference

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // Scroll to bottom
    }

    private void AddPlayerChat(string message)
    {
        GameObject newText = Instantiate(newTextPrefab, content);
        newText.GetComponent<TMP_Text>().text = message;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0; // Scroll to bottom
    }

    private IEnumerator SendToGemini(string prompt)
    {
        // Correct JSON Payload Format
        string jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";

        // Create the web request
        string fullUrl = apiUrl + apiKey;
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response
            string response = request.downloadHandler.text;
            Debug.Log(response);

            GeminiResponse responseFinal = JsonUtility.FromJson<GeminiResponse>(response);

            // Check if response contains valid text
            if (responseFinal != null && responseFinal.candidates.Length > 0 &&
                responseFinal.candidates[0].content.parts.Length > 0)
            {
                string text = responseFinal.candidates[0].content.parts[0].text;
                AddToChat($"Bot: {text}");
            }
            else
            {
                AddToChat("Bot: Sorry, I didn't understand that.");
            }
        }
        else
        {
            Debug.LogError("API Error: " + request.error);
            AddToChat("Bot: Sorry, something went wrong.");
        }
    }

    [System.Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
        public UsageMetadata usageMetadata;
    }

    [System.Serializable]
    public class Candidate
    {
        public Content content;
        public string finishReason;
    }

    [System.Serializable]
    public class Content
    {
        public Part[] parts;
        public string role;
    }

    [System.Serializable]
    public class Part
    {
        public string text;
    }

    [System.Serializable]
    public class UsageMetadata
    {
        public int promptTokenCount;
        public int candidatesTokenCount;
        public int totalTokenCount;
    }
}
