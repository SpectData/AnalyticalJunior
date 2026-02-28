using System;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCellUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Button button;

    private Action clickAction;

    void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (backgroundImage == null) backgroundImage = GetComponent<Image>();
        if (button != null)
            button.onClick.AddListener(() => clickAction?.Invoke());
    }

    public void SetState(Color bgColor, Color borderColor, bool interactable, Action onClick)
    {
        if (backgroundImage != null) backgroundImage.color = bgColor;
        if (borderImage != null) borderImage.color = borderColor;
        if (button != null) button.interactable = interactable;
        clickAction = onClick;
    }
}
