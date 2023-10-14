using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraDrop : ProjectBehaviour
{
    private bool done = false;
    private Vector3 vector = Vector3.zero;

    private void Update()
    {
        if (Game.PlayerDied && done == false)
        {
            done = true;
            vector = this.transform.position + new Vector3 (0, -1.1f, 0);
            this.transform.position = Vector3.MoveTowards(this.transform.position, vector, 9.5f * Time.deltaTime);
        }
        else if (Game.PlayerDied)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, vector, 9.5f * Time.deltaTime);
        }
    }
}
