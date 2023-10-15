using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : ProjectBehaviour
{
    [SerializeField] GameObject music;

    private void Awake()
    {
        ApplicationStarted();
        GameObject g = Instantiate(music);
        DontDestroyOnLoad(g);
        LoadNextSceneInBuildIndex();
    }
}
