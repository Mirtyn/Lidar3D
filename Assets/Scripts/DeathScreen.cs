
using UnityEngine;

public class DeathScreen : ProjectBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenPressed();
        }
    }

    public void ScreenPressed()
    {
        ReLoadCurrentLevel();
    }
}
