using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectBehaviour : MonoBehaviour
{
    public static GameManager Game;
    private static string saveDataPath = Application.dataPath + "/save.txt";
    public static SaveGame currentSaveGameData = new SaveGame();

    public static void ApplicationStarted()
    {
        Load();
    }

    public static void GameStart()
    {
        Game = new GameManager();
        Game.PlayerDied = false;
    }

    public static void LevelCompleted(int lvl)
    {
        currentSaveGameData.LvlsWon[lvl] = true;
    }

    public static void LoadNextSceneInBuildIndex()
    {
        Scene s = SceneManager.GetActiveScene();

        SceneManager.LoadScene(s.buildIndex + 1);
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

    public class SaveGame
    {
        public bool[] LvlsWon = new bool[]
        {
            false,
            false,
            false
        };
    }
}

