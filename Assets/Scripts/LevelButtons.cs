using UnityEngine;

public class LevelButtons : ProjectBehaviour
{
    private bool Unlocked = false;
    [SerializeField] private UnityEngine.UI.Button button;

    [SerializeField] private GameObject lockImage;
    [SerializeField] private UnityEngine.UI.Image highlightedGlow;
    [SerializeField] private UnityEngine.UI.Image frameGlow;
    [SerializeField] private UnityEngine.UI.Image backGlow;

    [SerializeField] private Color unlockedColor = new Color();
    [SerializeField] private Color lockedColor = new Color();

    private void Awake()
    {
        highlightedGlow.color = lockedColor;
        frameGlow.color = lockedColor;
        backGlow.color = lockedColor;
    }

    private void Update()
    {
        if (Unlocked)
        {
            highlightedGlow.color = unlockedColor;
            frameGlow.color = unlockedColor;
            backGlow.color = unlockedColor;
        }
    }

    public void SetActive()
    {
        Unlocked = true;
        lockImage.SetActive(false);
        button.interactable = true;
        highlightedGlow.color = unlockedColor;
        frameGlow.color = unlockedColor;
        backGlow.color = unlockedColor;
    }
}
