using UnityEngine;

public class Finish : ProjectBehaviour
{
    private GameObject player;
    public int Level = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            LevelCompleted(Level);
            LoadNextSceneInBuildIndex();
        }
        else if (other.transform.parent == player)
        {
            LevelCompleted(Level);
            LoadNextSceneInBuildIndex();
        }
    }
}
