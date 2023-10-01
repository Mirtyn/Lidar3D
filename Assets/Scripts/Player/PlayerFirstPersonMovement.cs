using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFirstPersonMovement : ProjectBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float playerWalkSpeed = 3f;
    [SerializeField] private float playerRunSpeed = 8f;
    [SerializeField] private float accelerationAndDeaccelerationSpeed = 8f;

    [SerializeField] private float jumpTimeOut;
}
