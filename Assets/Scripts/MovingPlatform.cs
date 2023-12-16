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
    [SerializeField] private float stopTime = 1.5f;
    private bool stopped = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCharacterController = player.GetComponent<CharacterController>();

        float rndNum = forceStartValue;

        if (!forcePlayOnStart )
        {
            rndNum = Random.Range(0f, maxDelay);
        }

        Invoke(nameof(Play), rndNum);

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
            if (!stopped)
            {
                var target = dir ? endPoint : startPoint;

                var prevPos = this.transform.position;

                this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, moveSpeed * Time.deltaTime);

                var nextPos = this.transform.position;
                diffrence = nextPos - prevPos;

                if (this.transform.position == target.transform.position)
                {
                    stopped = true;
                    Invoke(nameof(ChangeDir), stopTime);
                }

                if (playerInside)
                {
                    playerCharacterController.Move(diffrence);
                }
            }
        }
    }

    private void ChangeDir()
    {
        dir = !dir;
        stopped = false;
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
