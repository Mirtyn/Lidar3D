using UnityEngine;
using System;

public class Pixel : ProjectBehaviour
{
    [SerializeField] Material[] materials;

    public enum _VoxelColour
    {
        White,
        Red,
        Green,
        Blue,
        Magenta,
        Gray
    }

    public _VoxelColour VoxelColour;

    //private float rangeBeforeChange = 0.1f;

    public Transform Parent;
    public Vector3 Offset;
    public Quaternion RotOffset;

    private Transform pixelVisualTransform;
    public Vector3 VisualPositionWorldSpace;

    public float TimeDelta = 0f;
    public float MaxTimeDelta;
    private float minTime = 20f;
    private float maxTime = 30f;
    private float ATime = 0f;
    private Renderer renderer;

    private void OnEnable()
    {
        pixelVisualTransform = transform.GetChild(0);
        VisualPositionWorldSpace = pixelVisualTransform.position;

        MaxTimeDelta = UnityEngine.Random.Range(minTime, maxTime);
        TimeDelta = ATime;
        renderer = pixelVisualTransform.GetComponent<Renderer>();
    }

    public void SetMaterial()
    {
        renderer.material = VoxelColour switch
        {
            _VoxelColour.Red => materials[1],
            _VoxelColour.Blue => materials[2],
            _VoxelColour.Green => materials[3],
            _VoxelColour.Gray => materials[4],
            _VoxelColour.Magenta => materials[5],
            _ => materials[0],
        };
    }

    private Color glitchColor;

    private void Update()
    {
        if (!Game.GamePaused)
        {
            if (StickyVoxels)
            {
                // Position the object on top of the parent
                this.transform.position = Parent.position;
                // Set the rotation based on the parent and stored offset rotation
                this.transform.rotation = Parent.rotation * RotOffset;
                // Move the child back to the reference location
                this.transform.Translate(Offset);

                pixelVisualTransform.position = VisualPositionWorldSpace;

                Vector3 pos = new Vector3((float)Math.Round(this.transform.position.x, 1), (float)Math.Round(this.transform.position.y, 1), (float)Math.Round(this.transform.position.z, 1));

                if (VisualPositionWorldSpace != pos)
                {
                    VisualPositionWorldSpace = pos;
                }

                pixelVisualTransform.eulerAngles = Vector3.zero;
            }

            TimeDelta += Time.deltaTime;

            if (TimeDelta > MaxTimeDelta)
            {
                TimeDelta -= UnityEngine.Random.Range(2f, 5f);

                glitchColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0.4f, 1.2f));
                renderer.material.color = glitchColor;
                renderer.material.SetColor("_EmissionColor", glitchColor);
            }
        }
    }

    //public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    //{
    //    return Quaternion.Euler(angles) * (point - pivot) + pivot;
    //}
}

//private void Update()
//{
//    Vector3 prevPos = transform.localPosition;

//    this.transform.position = new Vector3((float)Math.Round(this.transform.position.x, 1), (float)Math.Round(this.transform.position.y, 1), (float)Math.Round(this.transform.position.z, 1));

//    if (transform.localPosition.x > (prevPos.x + 0.1f) || transform.localPosition.x < (prevPos.x - 0.1f))
//    {
//        transform.localPosition = new Vector3(prevPos.x, transform.localPosition.y, transform.localPosition.z);
//    }

//    if (transform.localPosition.y > (prevPos.y + 0.1f) || transform.localPosition.y < (prevPos.y - 0.1f))
//    {
//        transform.localPosition = new Vector3(transform.localPosition.x, prevPos.y, transform.localPosition.z);
//    }

//    if (transform.localPosition.z > (prevPos.z + 0.1f) || transform.localPosition.z < (prevPos.z - 0.1f))
//    {
//        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, prevPos.z);
//    }

//    this.transform.eulerAngles = Vector3.zero;
//}

