using UnityEngine;

public class ObjectHolder : ProjectBehaviour
{
    public Rigidbody AttachedRigidbody;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform playerCapsuleTransform;

    [SerializeField] private LayerMask pusheableObjectLayer;

    [SerializeField] private ForceMode forceMode;
    //[SerializeField] private float multiplier = 0.25f;
    //[SerializeField] private float speed = 1f;

    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private float minForce = 0f;
    [SerializeField] private float maxForce = 11f;
    [SerializeField] private float distanceDevider = 1.5f;
    [SerializeField] private float procentScaler = 0.6f;

    private float distanceOfCamera = 2.75f;
    private float distanceOfRay = 0.6f;

    private void Update()
    {
        this.transform.position = playerCameraTransform.position + playerCameraTransform.forward * distanceOfCamera;

        if (AttachedRigidbody != null)
        {
            if (Vector3.Distance(this.transform.position, AttachedRigidbody.transform.position) > 5f)
            {
                AttachedRigidbody = null;
            }
        }

        //joint.connectedAnchor = Vector3.Lerp(this.transform.position + this.transform.forward * 0.2f, joint.connectedAnchor, speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            if (AttachedRigidbody != null)
            {
                Vector3 dir = this.transform.position - AttachedRigidbody.transform.position;
                //Vector3 nextPos = AttachedRigidbody.transform.position + dir * multiplier;

                //

                AttachedRigidbody.transform.rotation = Quaternion.RotateTowards(AttachedRigidbody.transform.rotation, playerCapsuleTransform.rotation, rotateSpeed * Time.deltaTime);

                //

                //Vector3 center = AttachedRigidbody.transform.position;
                //Vector3 size = AttachedRigidbody.gameObject.GetComponent<BoxCollider>().size / 2;
                //Vector3 halfExtents = new Vector3(size.x * AttachedRigidbody.transform.localScale.x, size.y * AttachedRigidbody.transform.localScale.y, size.z * AttachedRigidbody.transform.localScale.z);
                //Quaternion orientation = AttachedRigidbody.transform.rotation;

                //if (Physics.BoxCast(center, halfExtents, dir, orientation, multiplier, mapLayer))
                //{
                //    ExtDebug.DrawBoxCastBox(center, halfExtents, orientation, dir, multiplier, Color.red);
                //    // dont move obj
                //}
                //else
                //{
                //    ExtDebug.DrawBoxCastBox(center, halfExtents, orientation, dir, multiplier, Color.red);
                //    // move obj
                //    AttachedRigidbody.transform.position = nextPos;
                //}

                //

                AttachedRigidbody.linearVelocity *= procentScaler;
                AttachedRigidbody.angularVelocity *= procentScaler;

                Vector3 force = Mathf.Lerp(minForce, maxForce, Vector3.Distance(this.transform.position, AttachedRigidbody.transform.position) / distanceDevider) * dir;
                Debug.DrawLine(AttachedRigidbody.transform.position, AttachedRigidbody.transform.position + force, Color.green, 0.2f);
                AttachedRigidbody.AddForce(force, forceMode);

                //

                //AttachedRigidbody.MovePosition(dir * multiplier);

                //

                //AttachedRigidbody.velocity = Vector3.zero;
                //AttachedRigidbody.transform.position = Vector3.MoveTowards(AttachedRigidbody.transform.position, nextPos, speed * Time.deltaTime);

                //

                Physics.Raycast(playerCapsuleTransform.position, Vector3.down, out RaycastHit hit, distanceOfRay, pusheableObjectLayer);

                if (hit.transform != null)
                {
                    if (hit.collider.attachedRigidbody == AttachedRigidbody)
                    {
                        AttachedRigidbody = null;
                    }
                }
            }
        }
    }
}

//public static class ExtDebug
//{
//    //Draws just the box at where it is currently hitting.
//    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
//    {
//        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
//        DrawBox(origin, halfExtents, orientation, color);
//    }

//    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
//    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
//    {
//        direction.Normalize();
//        Box bottomBox = new Box(origin, halfExtents, orientation);
//        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

//        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
//        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
//        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
//        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
//        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
//        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
//        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
//        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

//        DrawBox(bottomBox, color);
//        DrawBox(topBox, color);
//    }

//    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
//    {
//        DrawBox(new Box(origin, halfExtents, orientation), color);
//    }
//    public static void DrawBox(Box box, Color color)
//    {
//        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
//        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
//        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
//        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

//        Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
//        Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
//        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
//        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

//        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
//        Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
//        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
//        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
//    }

//    public struct Box
//    {
//        public Vector3 localFrontTopLeft { get; private set; }
//        public Vector3 localFrontTopRight { get; private set; }
//        public Vector3 localFrontBottomLeft { get; private set; }
//        public Vector3 localFrontBottomRight { get; private set; }
//        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
//        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
//        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
//        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

//        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
//        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
//        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
//        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
//        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
//        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
//        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
//        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

//        public Vector3 origin { get; private set; }

//        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
//        {
//            Rotate(orientation);
//        }
//        public Box(Vector3 origin, Vector3 halfExtents)
//        {
//            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
//            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
//            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
//            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

//            this.origin = origin;
//        }


//        public void Rotate(Quaternion orientation)
//        {
//            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
//            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
//            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
//            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
//        }
//    }

//    //This should work for all cast types
//    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
//    {
//        return origin + (direction.normalized * hitInfoDistance);
//    }

//    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
//    {
//        Vector3 direction = point - pivot;
//        return pivot + rotation * direction;
//    }
//}