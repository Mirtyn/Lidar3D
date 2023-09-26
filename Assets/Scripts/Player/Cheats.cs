using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [SerializeField] private GameObject lights;

    void Update()
    {
        if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.M))
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("Lights Cheat");
                lights.SetActive(!lights.activeSelf);
            }
        }
    }
}
