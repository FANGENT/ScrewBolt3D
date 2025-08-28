using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SplashScript : MonoBehaviour
{
    public float delayTime = 5f; // Time before loading next scene
    public string sceneToLoad = "MainMenu"; // Scene name to load
    public Slider loadingSlider; // Reference to UI Slider

    public TextMeshProUGUI percentageText;

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

            // Update slider and percentage text
            loadingSlider.value = progress;
            percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
