using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectBehaviour : MonoBehaviour
{
    public static GameManager Game;
    public static float PlayerSensetivity = 2f;

    public static void GameStart()
    {
        Game = new GameManager();
    }
}
