
using UnityEngine;
using UnityEngine.InputSystem;

public class PausedMenu : ProjectBehaviour
{
    private Player player;

    private void OnEnable()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void ResumeButton()
    {
        this.gameObject.SetActive(false);
     
        player._input.cursorInputForLook = true;
        player._input.cursorLocked = true;
        ResumeGame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitButton()
    {
        LoadLevelByIndex(1);
    }
}
