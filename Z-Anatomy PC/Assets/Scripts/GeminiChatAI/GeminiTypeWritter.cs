using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class GeminiTypewritter : MonoBehaviour
{
    public float delay = 0.05f; // Delay between each character
    public string fullText; // The full text to display
    private Coroutine typingCoroutine;

    // Starts the typing effect with the given text
    public void StartTyping(string text)
    {
        fullText = AddLineBreaks(text, 30); // Add line breaks after 30 characters

        // Stop any ongoing typing coroutine before starting a new one
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText());
    }

    // Coroutine to type out the text with a delay between each character
    private IEnumerator TypeText()
    {
        TMP_Text textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("GeminiTypewritter: No TMP_Text component found!");
            yield break;
        }

        textComponent.text = ""; // Clear the text initially

        foreach (char letter in fullText)
        {
            textComponent.text += letter; // Add one character at a time
            yield return new WaitForSeconds(delay); // Wait for the specified delay
        }
    }

    // Method to add line breaks in the text after a specific number of characters
    private string AddLineBreaks(string text, int maxCharactersPerLine)
    {
        string processedText = "";
        int currentLineLength = 0;

        foreach (char c in text)
        {
            processedText += c;
            currentLineLength++;

            // Add a new line if we've reached the max character limit for the line
            if (currentLineLength >= maxCharactersPerLine && c == ' ')
            {
                processedText += "\n";
                currentLineLength = 0;
            }
        }

        return processedText;
    }
}
