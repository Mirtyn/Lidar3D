using UnityEngine;

public class MovingPlatform : ProjectBehaviour
{
    private GameObject player;
    private CharacterController playerCharacterController;
    [SerializeField] private GameObject endPoint;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private bool dir = false;

    [SerializeField] private float moveSpeed = 1.25f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private Vector3 diffrence = Vector3.zero;
    private bool playerInside = false;

    [SerializeField] private bool forcePlayOnStart = false;
    [SerializeField] private float forceStartValue = 0f;
    [SerializeField] private float maxDelay = 5f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCharacterController = player.GetComponent<CharacterController>();

        float rndNum = forceStartValue;

        if (!forcePlayOnStart )
        {
            rndNum = Random.Range(0f, maxDelay);
        }

        Invoke("Play", rndNum);

        startPoint.SetActive(false);
        endPoint.SetActive(false);
    }

    private void Play()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void Update()
    {
        if (!Game.GamePaused)
        {
            var target = dir ? endPoint : startPoint;

            var prevPos = this.transform.position;

            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, moveSpeed * Time.deltaTime);

            var nextPos = this.transform.position;
            diffrence = nextPos - prevPos;

            if (this.transform.position == target.transform.position)
            {
                dir = !dir;
            }

            if (playerInside)
            {
                playerCharacterController.Move(diffrence);
            }
        }
    }

    public void OpenDoor()
    {
        if (!Game.GamePaused)
        {
            dir = true;
        }
    }

    public void CloseDoor()
    {
        if (!Game.GamePaused)
        {
            dir = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInside = true;
        }
        else if (other.transform.parent == player)
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInside = false;
        }
        else if (other.transform.parent == player)
        {
            playerInside = false;
        }
    }
}
