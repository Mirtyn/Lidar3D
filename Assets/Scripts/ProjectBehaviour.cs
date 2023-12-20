using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectBehaviour : MonoBehaviour
{
    public static GameManager Game;
    private static string saveDataPath = Application.dataPath + "/save.txt";
    public static SaveGame currentSaveGameData = new SaveGame();
    public static float GameSpeed = 1.0f;

    public static bool StickyVoxels = true;

    public static void ApplicationStarted()
    {
        Load();
    }

    public static void PauseGame()
    {
        Time.timeScale = 0f;
        Game.GamePaused = true;
    }

    public static void ResumeGame()
    {
        Time.timeScale = GameSpeed;
        Game.GamePaused = false;
    }

    public static void DisableInput()
    {
        Game.CanUseInput = false;
    }

    public static void EnableInput()
    {
        Game.CanUseInput = true;
    }

    public static void GameStart(bool mainMenu)
    {
        Game = new GameManager();

        if (!mainMenu)
        {
            EnableInput();
            ResumeGame();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Game.PlayerDied = false;
        }
    }

    public static void ReLoadCurrentLevel()
    {
        Scene s = SceneManager.GetActiveScene();

        SceneManager.LoadScene(s.buildIndex);
    }

    public static void LevelCompleted(int lvl)
    {
        currentSaveGameData.LvlsWon[lvl] = true;
    }

    public static void LoadLevelByIndex(int lvlIndex)
    {
        SceneManager.LoadScene(lvlIndex);
    }

    public static void LoadNextSceneInBuildIndex()
    {
        Scene s = SceneManager.GetActiveScene();

        if (s.buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(s.buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public static void Save()
    {
        string jsonString = JsonUtility.ToJson(currentSaveGameData);
        File.WriteAllText(saveDataPath, jsonString);
    }

    public static void Load()
    {
        if (File.Exists(saveDataPath))
        {
            string saveString = File.ReadAllText(saveDataPath);
            currentSaveGameData = JsonUtility.FromJson<SaveGame>(saveString);
        }
    }

    public static void QuitGame()
    {
        Save();
        Application.Quit();
    }

    public static void SetGameSpeed(float value)
    {
        GameSpeed = value;

        if (!Game.GamePaused)
        {
            Time.timeScale = GameSpeed;
        }
    }
}

public class SaveGame
{
    public bool[] LvlsWon = new bool[]
    {
        false,
        false,
        false,
        false
    };
}
