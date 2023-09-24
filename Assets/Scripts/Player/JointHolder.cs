using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointHolder : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;

    private float distanceOfCamera = 1.5f;

    private void Update()
    {
        this.transform.position = playerCameraTransform.position + playerCameraTransform.forward * distanceOfCamera;

        //joint.connectedAnchor = Vector3.Lerp(this.transform.position + this.transform.forward * 0.2f, joint.connectedAnchor, speed * Time.deltaTime);
    }
}
