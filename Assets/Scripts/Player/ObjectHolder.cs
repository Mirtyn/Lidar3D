using UnityEngine;

public class ObjectHolder : ProjectBehaviour
{
    public Rigidbody AttachedRigidbody;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerCapsuleTransform;

    [SerializeField] private LayerMask pusheableObjectLayer;

    [SerializeField] private ForceMode forceMode;
    [SerializeField] private float multiplier = 0.25f;

    private float distanceOfCamera = 2f;
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
            Vector3 dir = this.transform.position - AttachedRigidbody.transform.position;

            AttachedRigidbody.AddForce(dir * multiplier, forceMode);

            Physics.Raycast(playerCapsuleTransform.position, Vector3.down, out RaycastHit hit, distanceOfRay, pusheableObjectLayer);

            if (hit.transform != null)
            {
                if (TryGetComponent<SpringJoint>(out SpringJoint sJ))
                {
                    if (hit.collider.attachedRigidbody == sJ.connectedBody)
                    {
                        Destroy(sJ);
                    }
                }
            }
        }
    }
}