using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    private bool showConsole = false;

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

        GUI.Box(new Rect(0, y, Screen.width, 40 * Screen.height / 1080), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);
    }
}
