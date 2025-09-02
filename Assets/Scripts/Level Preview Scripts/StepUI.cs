using UnityEngine;
using UnityEngine.UI;

public class StepUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private Image check;
    [SerializeField] private Image connector;

    public void Setup(Sprite iconSprite, bool isCompleted, bool showConnector, bool connectorCompleted, Color doneColor, Color todoColor)
    {
        if (icon) icon.sprite = iconSprite;
        if (check) check.enabled = isCompleted;

        if (connector)
        {
            connector.gameObject.SetActive(showConnector);
            if (showConnector) connector.color = connectorCompleted ? doneColor : todoColor;
        }
    }
}
