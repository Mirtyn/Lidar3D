using UnityEngine;

public class MainMenu : ProjectBehaviour
{
    public LevelButtons FirstButton;
    public LevelButtons[] LevelButtons;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        FirstButton.SetActive();

        for (int i = 0; i < currentSaveGameData.LvlsWon.Length; i++)
        {
            if (currentSaveGameData.LvlsWon[i] == true)
            {
                if (i < LevelButtons.Length)
                {
                    LevelButtons[i].SetActive();
                }
            }
        }
    }

    public void LoadLevel(int level)
    {
        LoadLevelByIndex(level + 1);
    }

    public void QuitButtonPressed()
    {
        QuitGame();
    }
}
