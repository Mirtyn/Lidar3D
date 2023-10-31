using UnityEngine;
using System;

public class Pixel : ProjectBehaviour
{
    public bool Permanent = true;

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

    private void OnEnable()
    {
        pixelVisualTransform = transform.GetChild(0);
        VisualPositionWorldSpace = pixelVisualTransform.position;
    }

    private void Update()
    {
        if (!Game.GamePaused)
        {
            if (Permanent)
            {
                if (StickyVoxels)
                {
                    // Position the object on top of the parent
                    this.transform.position = Parent.position;
                    // Set the rotation based on the parent and stored offset rotation
                    this.transform.rotation = Parent.rotation * RotOffset;
                    // Move the child back to the reference location
                    this.transform.Translate(Offset);

                    //this.transform.position = RotatePointAroundPivot(this.transform.position, Parent.position, Ofset);

                    //this.transform.rotation = Parent.rotation;

                    //this.transform.position = Parent.position + Ofset;

                    //this.transform.RotateAround(Parent.position, Vector3.right, Ofset.x);
                    //this.transform.RotateAround(Parent.position, Vector3.up, Ofset.);
                    //this.transform.RotateAround(Parent.position, Vector3.forward, Ofset.z);

                    pixelVisualTransform.position = VisualPositionWorldSpace;

                    Vector3 pos = new Vector3((float)Math.Round(this.transform.position.x, 1), (float)Math.Round(this.transform.position.y, 1), (float)Math.Round(this.transform.position.z, 1));

                    if (VisualPositionWorldSpace != pos)
                    {
                        VisualPositionWorldSpace = pos;
                    }

                    pixelVisualTransform.eulerAngles = Vector3.zero;
                }
            }
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    private Vector3 RotatePointAroundPivot2(Vector3 point, Vector3 pivot, Vector3 angles) 
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles)* dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
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

