using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    private bool showConsole = false;

    private string input;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            OnToggleConsole();
        }
    }

    public void OnToggleConsole()
    {
        showConsole = !showConsole;
    }

    private void OnGUI()
    {
        if (!showConsole) { return; }

        float y = 0f;

        GUI.Box(new Rect(0f, y, Screen.width, 30f), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        input = GUI.TextField(new Rect(-10f, y + -5f, Screen.width - 20f, 20f), input);


    }
}
