using UnityEngine;

public class Finish : ProjectBehaviour
{
    private GameObject player;
    public int Level = 0;
    private GameObject finishScreen;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        finishScreen = GameObject.FindGameObjectWithTag("FinishScreen");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            LevelCompleted(Level);
            finishScreen.GetComponent<VictoryScreen>().RunEnd();
        }
        else if (other.transform.parent == player)
        {
            LevelCompleted(Level);
            finishScreen.GetComponent<VictoryScreen>().RunEnd();
        }
    }
}
