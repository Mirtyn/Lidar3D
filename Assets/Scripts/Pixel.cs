using UnityEngine;
using System;

public class Pixel : ProjectBehaviour
{
    public enum _PixelColour
    {
        White,
        Red,
        Green,
        Blue,
        Magenta,
        Gray
    }

    public _PixelColour PixelColour;

    //private float rangeBeforeChange = 0.1f;

    private Transform pixelVisualTransform;
    public Vector3 VisualPositionWorldSpace;

    private void OnEnable()
    {
        pixelVisualTransform = transform.GetChild(0);
        VisualPositionWorldSpace = pixelVisualTransform.position;
    }

    private void Update()
    {
        pixelVisualTransform.position = VisualPositionWorldSpace;

        Vector3 pos = new Vector3((float)Math.Round(this.transform.position.x, 1), (float)Math.Round(this.transform.position.y, 1), (float)Math.Round(this.transform.position.z, 1));

        if (VisualPositionWorldSpace != pos)
        {
            VisualPositionWorldSpace = pos;
        }

        pixelVisualTransform.eulerAngles = Vector3.zero;
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
}
