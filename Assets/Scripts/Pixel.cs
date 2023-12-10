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
    private bool fadePixel = false;
    private float lowerAmount = 0f;
    private Vector3 moveDir = Vector3.zero;

    public float ChangeDirDelta = 0f;
    public float MaxChangeDirDelta;

    private float minChangeDir = 0.37f;
    private float maxChangeDir = 0.68f;

    private void OnEnable()
    {
        pixelVisualTransform = transform.GetChild(0);
        VisualPositionWorldSpace = pixelVisualTransform.position;

        MaxTimeDelta = UnityEngine.Random.Range(minTime, maxTime);
        TimeDelta = ATime;
        renderer = pixelVisualTransform.GetComponent<Renderer>();
        fadePixel = false;
        lowerAmount = 0f;
        ChangeDirDelta = 0f;
        MaxChangeDirDelta = UnityEngine.Random.Range(minChangeDir, maxChangeDir);
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
                fadePixel = true;
                TimeDelta -= UnityEngine.Random.Range(2f, 5f);

                //glitchColor = new Color(renderer.material.color.r + UnityEngine.Random.Range(-0.2f, 0.2f), renderer.material.color.g + UnityEngine.Random.Range(-0.2f, 0.2f), renderer.material.color.b + UnityEngine.Random.Range(-0.2f, 0.2f));
                glitchColor = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b);
                renderer.material.color = glitchColor;
                renderer.material.SetColor("_EmissionColor", glitchColor - new Color(lowerAmount, lowerAmount, lowerAmount));
            }

            if(fadePixel)
            {
                lowerAmount += Time.deltaTime / 7.75f;
                renderer.material.SetColor("_EmissionColor", glitchColor - new Color(lowerAmount, lowerAmount, lowerAmount));

                if (renderer.material.GetColor("_EmissionColor").r <= 0 && renderer.material.GetColor("_EmissionColor").g <= 0 && renderer.material.GetColor("_EmissionColor").b <= 0)
                {
                    this.gameObject.SetActive(false);
                }

                this.transform.Translate(moveDir.normalized * Time.deltaTime);

                ChangeDirDelta += Time.deltaTime;
            }

            if (ChangeDirDelta > MaxChangeDirDelta)
            {
                ChangeDirDelta = 0;
                MaxChangeDirDelta = UnityEngine.Random.Range(minChangeDir, maxChangeDir);
                moveDir = new Vector3(UnityEngine.Random.Range(-1f, 1f) * lowerAmount, -Mathf.Pow(lowerAmount, 3f) -(lowerAmount / 2), UnityEngine.Random.Range(-1f, 1f) * lowerAmount);
            }
        }
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

