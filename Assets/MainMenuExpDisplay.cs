using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuExpDisplay : MonoBehaviour
{
    public Text expText;
    public Slider expProgressBar;
    public Image badgeImage; // Single Image to display the badge
    public Sprite[] levelBadges; // Array of badge images for each level

    private void OnEnable()
    {
        StartCoroutine(WaitForExpManager());
    }

    private IEnumerator WaitForExpManager()
    {
        // Wait until ExpManager is initialized
        while (ExpManager.Instance == null)
        {
            yield return null;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (expText == null || expProgressBar == null || badgeImage == null)
        {
            Debug.LogWarning("UI components are not assigned in the Inspector.");
            return;
        }

        int currentExp = ExpManager.Instance.Experience;
        int currentCap = ExpManager.Instance.ExpCap;
        int currentLevel = ExpManager.Instance.Level;

        // Update experience text
        expText.text = $"EXP: {currentExp}/{currentCap}";

        // Update progress bar
        expProgressBar.maxValue = currentCap;
        expProgressBar.value = currentExp;

        // Update badge image based on the current level
        if (currentLevel - 1 < levelBadges.Length && currentLevel > 0)
        {
            badgeImage.sprite = levelBadges[currentLevel - 1];
        }
        else
        {
            Debug.LogWarning("No badge image found for this level.");
        }
    }
}
