using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SplashScript : MonoBehaviour
{
    public float delayTime = 5f; // Time before loading next scene
    public string sceneToLoad = "MainMenu"; // Scene name to load
    public Image loadingFillImage; // Reference to UI Image (must be "Filled" type)

    public TextMeshProUGUI percentageText;

    public string privacyPolicyURL = "https://fangent.com"; // Replace with your actual URL

    void Start()
    {
        Application.targetFrameRate = 60; // Set target frame rate to 60 FPS
        StartCoroutine(LoadSceneWithDelay());
    }

    IEnumerator LoadSceneWithDelay()
    {
        float elapsedTime = 0f;

        while (elapsedTime < delayTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / delayTime);

            // Update image fill and percentage text
            if (loadingFillImage != null)
                loadingFillImage.fillAmount = progress;

            if (percentageText != null)
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }


    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyURL);
    }
}
