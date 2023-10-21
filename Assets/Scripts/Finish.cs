using Unity.VisualScripting;
using UnityEngine;

public class Finish : ProjectBehaviour
{
    private GameObject player;
    [SerializeField] private int level = 0;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            LevelCompleted(level);
            LoadNextSceneInBuildIndex();
        }
        else if (other.transform.parent == player)
        {
            LevelCompleted(level);
            LoadNextSceneInBuildIndex();
        }
    }
}
