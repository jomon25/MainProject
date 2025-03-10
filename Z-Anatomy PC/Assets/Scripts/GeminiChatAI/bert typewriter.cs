using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class GeminiTypewritte : MonoBehaviour
{
    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private int maxLineLength = 60;

    private TMP_Text textComponent;
    private Coroutine typingCoroutine;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.text = string.Empty;
    }

    public void StartTyping(string message)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(FormatMessage(message)));
    }

    IEnumerator TypeText(string formattedText)
    {
        textComponent.text = string.Empty;

        foreach (char c in formattedText)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(charDelay);
        }
    }

    string FormatMessage(string originalText)
    {
        string[] words = originalText.Split(' ');
        string currentLine = "";
        string result = "";

        foreach (string word in words)
        {
            if ((currentLine + word).Length > maxLineLength)
            {
                result += currentLine.TrimEnd() + "\n";
                currentLine = "";
            }
            currentLine += word + " ";
        }

        return (result + currentLine).TrimEnd();
    }
}