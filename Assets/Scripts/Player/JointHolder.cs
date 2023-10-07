using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointHolder : ProjectBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerCapsuleTransform;

    [SerializeField] private LayerMask pusheableObjectLayer;

    private float distanceOfCamera = 1.5f;
    private float distanceOfRay = 0.6f;


    private void Update()
    {
        this.transform.position = playerCameraTransform.position + playerCameraTransform.forward * distanceOfCamera;

        //joint.connectedAnchor = Vector3.Lerp(this.transform.position + this.transform.forward * 0.2f, joint.connectedAnchor, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            Physics.Raycast(playerCapsuleTransform.position, Vector3.down, out RaycastHit hit, distanceOfRay, pusheableObjectLayer);

            if (hit.transform != null)
            {
                if (hit.collider.attachedRigidbody == this.GetComponent<SpringJoint>().connectedBody)
                {
                    Destroy(this.GetComponent<SpringJoint>());
                }
            }
        }
    }
}