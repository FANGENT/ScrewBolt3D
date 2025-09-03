using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BottomNavController : MonoBehaviour
{
    public RectTransform homePanel;
    public RectTransform shopPanel;

    [Header("Buttons")]
    public Button homeButton;
    public Button shopButton;

    private float screenWidth;

    void Start()
    {
        screenWidth = Screen.width;
        Debug.Log("Screen Width: " + screenWidth);
        if(Screen.width < 1080)
        {
            screenWidth = 1080; // Set a minimum width for smaller screens
        }
        // Ensure starting positions
        homePanel.anchoredPosition = Vector2.zero;
        shopPanel.anchoredPosition = new Vector2(-screenWidth, 0);


        homeButton.onClick.AddListener(ShowHome);
        shopButton.onClick.AddListener(ShowShop);
    }

    public void ShowHome()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        homePanel.DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);
        shopPanel.DOAnchorPos(new Vector2(-screenWidth, 0), 0.4f).SetEase(Ease.OutCubic);

        //--A
        shopButton.GetComponent<Image>().enabled = false;
        homeButton.GetComponent<Image>().enabled = true;

        shopButton.transform.GetChild(0).gameObject.SetActive(true);
        shopButton.transform.GetChild(1).gameObject.SetActive(false);

        homeButton.transform.GetChild(0).gameObject.SetActive(false);
        homeButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ShowShop()
    {
        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("Button Click");
        }

        homePanel.DOAnchorPos(new Vector2(screenWidth, 0), 0.4f).SetEase(Ease.OutCubic);
        shopPanel.DOAnchorPos(Vector2.zero, 0.4f).SetEase(Ease.OutCubic);

        //--A
        homeButton.GetComponent<Image>().enabled = false;
        shopButton.GetComponent<Image>().enabled = true;


        homeButton.transform.GetChild(0).gameObject.SetActive(true);
        homeButton.transform.GetChild(1).gameObject.SetActive(false);

        shopButton.transform.GetChild(0).gameObject.SetActive(false);
        shopButton.transform.GetChild(1).gameObject.SetActive(true);
    }
}
