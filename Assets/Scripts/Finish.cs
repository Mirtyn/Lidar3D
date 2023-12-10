using Unity.VisualScripting;
using UnityEngine;

public class Finish : ProjectBehaviour
{
    public static Finish Current;
    private GameObject player;
    public int Level = 0;
    [SerializeField] GameObject FinishScreen;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Current = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            FinishScreen.SetActive(true);
        }
        else if (other.transform.parent == player)
        {
            FinishScreen.SetActive(true);
        }
    }
}
