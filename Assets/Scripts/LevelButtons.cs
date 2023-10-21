using UnityEngine;

public class LevelButtons : ProjectBehaviour
{
    [SerializeField] private UnityEngine.UI.Button button;

    [SerializeField] private GameObject lockImage;
    [SerializeField] private UnityEngine.UI.Image highlightedGlow;

    [SerializeField] private Color unlockedColor = new Color();
    [SerializeField] private Color lockedColor = new Color();

    private void Awake()
    {
        highlightedGlow.color = lockedColor;
    }

    public void SetActive()
    {
        lockImage.SetActive(false);
        button.interactable = true;
        highlightedGlow.color = unlockedColor;
    }
}
