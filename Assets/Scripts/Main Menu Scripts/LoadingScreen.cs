using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text loadingText;  // Text component for "LOADING"
    public Image logo;            // Image component for your logo
    public float wiggleIntensity = 5f;  // How much each letter wiggles
    public float wiggleSpeed = 0.1f;    // Speed of wiggle for each letter
    public float rotationSpeed = 100f;  // Speed of logo rotation

    private string originalText = "LOADING";

    void Start()
    {
        StartCoroutine(WiggleText());
    }

    void Update()
    {
        // Rotate the logo continuously
        logo.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void SetLoadingText(string newText)
    {
        originalText = newText;  // Update the original text
    }

    private IEnumerator WiggleText()
    {
        while (true)
        {
            for (int i = 0; i < originalText.Length; i++)
            {
                // Reset to original text
                loadingText.text = "";

                for (int j = 0; j < originalText.Length; j++)
                {
                    char letter = originalText[j];
                    if (j == i)
                    {
                        // Apply a wiggle effect to the current letter
                        float offsetX = Mathf.Sin(Time.time * wiggleSpeed + j) * wiggleIntensity;
                        loadingText.text += $"<voffset={offsetX}px>{letter}</voffset>";
                    }
                    else
                    {
                        loadingText.text += letter;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
