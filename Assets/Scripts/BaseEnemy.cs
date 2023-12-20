using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : ProjectBehaviour
{
    private GameObject player;
    private Transform playerTransform;
    private Transform thisTransform;
    private Collider playerCollider;
    private NavMeshAgent agent;
    private float viewDistance = 13.5f;

    [SerializeField] private LayerMask mapMask;
    [SerializeField] private bool ranged;

    private bool change = true;
    private float changeDelta = 0f;
    private float maxChange = 0.6f;

    private float updateTimerDelta = 0f;
    private float maxUpdateTimer = 0.15f;

    [SerializeField] private float footStepSoundDelay = 1.6f;
    [SerializeField] private float currentFootStepSoundDelay;

    [SerializeField] private AudioSource feetAudioSource;

    [SerializeField] private AudioClip[] footstepAudioClips;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        thisTransform = this.transform;
        playerCollider = player.GetComponentInChildren<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        currentFootStepSoundDelay = footStepSoundDelay;
    }

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            updateTimerDelta += Time.deltaTime;

            if (updateTimerDelta > maxUpdateTimer)
            {
                updateTimerDelta = 0;

                maxUpdateTimer = Random.Range(0.09f, 0.22f);

                var g = Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), new Vector2(thisTransform.position.x, thisTransform.position.z));

                if (g < viewDistance)
                {
                    Debug.DrawRay(thisTransform.position + thisTransform.up, (playerTransform.position - thisTransform.position).normalized * viewDistance, Color.blue, 5f);
                    if (Physics.Raycast(thisTransform.position + thisTransform.up, (playerTransform.position - thisTransform.position).normalized, out RaycastHit hit, viewDistance, mapMask))
                    {
                        if (hit.collider == playerCollider)
                        {
                            if (ranged)
                            {
                                RangedAttack();
                            }
                            else
                            {
                                Attack();
                            }
                        }
                        else
                        {
                            Idle();
                        }
                    }
                    else
                    {
                        Idle();
                    }
                }
                else
                {
                    Idle();
                }
            }

            if (!change)
            {
                changeDelta += Time.deltaTime;

                if (changeDelta > maxChange)
                {
                    changeDelta = 0;

                    maxChange = Random.Range(0.8f, 2.5f);

                    change = true;
                }
            }

            currentFootStepSoundDelay -= agent.velocity.magnitude * agent.speed * Time.deltaTime;

            if (currentFootStepSoundDelay <= 0)
            {
                currentFootStepSoundDelay = footStepSoundDelay;

                var rndNum = Random.Range(0, footstepAudioClips.Length);

                feetAudioSource.clip = footstepAudioClips[rndNum];
                feetAudioSource.Play();
            }
        }
    }

    private void Attack()
    {
        agent.destination = playerTransform.position;

        Debug.DrawLine(thisTransform.position + thisTransform.up, playerTransform.position, Color.red, 3f);

        change = false;
    }

    private void RangedAttack()
    {


        change = false;
    }

    private void Idle()
    {
        if (change)
        {
            var i = Random.Range(0.5f, 6f);
            var v = Random.insideUnitCircle * i;
            var v3 = new Vector3(v.x, 0, v.y);

            agent.SetDestination(v3 + thisTransform.position);

            Debug.DrawLine(thisTransform.position + thisTransform.up, v3 + thisTransform.position, Color.green, 3f);

            change = false;
        }
    }
}
