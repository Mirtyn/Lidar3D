using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : ProjectBehaviour
{
    private const string PUSHEABLE_OBJECT_TAG = "PusheableObject";
    private const string BUTTON_TAG = "Button";
    private const string DOOR_TAG = "Door";
    private const string ENEMY_TAG = "Enemy";

    [SerializeField] private Camera mainCamera;

    private float maxRaycastDistance = 1000f;

    [SerializeField] Transform pixelsParent;

    [SerializeField] GameObject pixelWhite;
    [SerializeField] GameObject pixelBlue;
    [SerializeField] GameObject pixelRed;
    [SerializeField] GameObject pixelMagenta;
    [SerializeField] GameObject pixelGray;

    private List<GameObject> pixels = new List<GameObject>();

    bool drawPictureNextFixedUpdate = false;
    bool tryGrabObjectNextFixedUpdate = false;
    bool draw1RandomDirLineFixedUpdate = false;
    bool drawDirLineForPixelRemovalFixedUpdate = false;

    [SerializeField] private LayerMask mapMask;
    [SerializeField] private LayerMask grabeableObjectMask;
    [SerializeField] private LayerMask pixelMask;

    [SerializeField] private SpringJoint jointData;
    [SerializeField] private SpringJoint joint;
    private Transform jointTransform;

    RaycastHit lastHitObject;


    private void Awake()
    {
        ProjectBehaviour.GameStart();
        //jointData = jointData.
        //jointData.autoConfigureConnectedAnchor = true;
        //jointData.breakForce = 20f;
        //jointData.enableCollision = false;
        //jointData.enablePreprocessing = true;
        //jointData.maxDistance = 0;
        //jointData.minDistance = 0;
        jointTransform = joint.gameObject.transform;
    }

    private void Update()
    {
        if (joint == null)
        {
            grabbedSomeThing = false;
            lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectLetGoOf();

            joint = jointTransform.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = jointData.autoConfigureConnectedAnchor;
            joint.breakForce = jointData.breakForce;
            joint.breakTorque = jointData.breakTorque;
            joint.damper = jointData.damper;
            joint.enableCollision = jointData.enableCollision;
            joint.enablePreprocessing = jointData.enablePreprocessing;
            joint.massScale = jointData.massScale;
            joint.maxDistance = jointData.maxDistance;
            joint.minDistance = jointData.minDistance;
            joint.connectedMassScale = jointData.connectedMassScale;
            joint.spring = jointData.spring;
            joint = jointTransform.gameObject.GetComponent<SpringJoint>();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Game.GamePaused = !Game.GamePaused;
        }

        if (!Game.GamePaused)
        {
            CheckForPlayerInput();
        }
        //Debug.Log(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            if (drawPictureNextFixedUpdate)
            {
                drawPictureNextFixedUpdate = false;

                DrawPicture();
            }

            if (draw1RandomDirLineFixedUpdate)
            {
                draw1RandomDirLineFixedUpdate = false;

                Draw1RandomDirLine();
            }

            if (drawDirLineForPixelRemovalFixedUpdate)
            {
                drawDirLineForPixelRemovalFixedUpdate = false;

                DrawRaysForPixelRemoval();
            }

            if (tryGrabObjectNextFixedUpdate)
            {
                tryGrabObjectNextFixedUpdate = false;

                TryGrabObject();
            }
        }
    }

    private void CheckForPlayerInput()
    {
        if (Game.CanUseInput)
        {
            if (Input.GetMouseButton(0))
            {
                draw1RandomDirLineFixedUpdate = true;
            }
            if (Input.GetMouseButton(1))
            {
                drawDirLineForPixelRemovalFixedUpdate = true;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                drawPictureNextFixedUpdate = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                tryGrabObjectNextFixedUpdate = true;
            }
        }
    }

    bool grabbedSomeThing = false;

    private void TryGrabObject()
    {
        if (grabbedSomeThing == false)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 3, grabeableObjectMask))
            {
                lastHitObject = hit;
                Debug.DrawLine(mainCamera.transform.position, mainCamera.transform.position + (mainCamera.transform.forward * 3), Color.magenta, 2f);

                lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectHeld();

                grabbedSomeThing = true;
                joint.connectedBody = hit.rigidbody;
            }
        }
        else
        {
            lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectLetGoOf();

            grabbedSomeThing = false;
            joint.connectedBody = null;
        }
    }

    int timesDrawRaysForPixelRemoval = 8;
    private void DrawRaysForPixelRemoval()
    {
        for (int i = 0; i < timesDrawRaysForPixelRemoval; i++)
        {
            Physics.Raycast(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.15f, 0.15f), mainCamera.transform.forward.y + Random.Range(-0.15f, 0.15f), mainCamera.transform.forward.z + Random.Range(-0.15f, 0.15f)), out RaycastHit hit, maxRaycastDistance, pixelMask);

            if (hit.transform != null)
            {
                RemovePixel(hit.transform.gameObject);
            }
        }
    }

    private void Draw1RandomDirLine()
    {
        List<RaycastHit> hit = new List<RaycastHit>();
        hit = Physics.RaycastAll(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.2f, 0.2f), mainCamera.transform.forward.y + Random.Range(-0.2f, 0.2f), mainCamera.transform.forward.z + Random.Range(-0.2f, 0.2f)), maxRaycastDistance, mapMask).ToList();

        CheckPixelColour(hit);
    }

    float pictureSizeX = 800f * Screen.width / 1920;
    float pictureSizeY = 800f * Screen.height / 1080;

    int pixelsInPictureOn1Line = 10;

    float distanceBetweenPixelsX;
    float distanceBetweenPixelsY;

    Vector3 screenPos = Vector3.zero;


    int i = 0;
    int j = 0;

    float timeInBetweenPicturePixelDrawn = 0.015f;

    private void DrawPicture()
    {
        distanceBetweenPixelsX = pictureSizeX / pixelsInPictureOn1Line;
        distanceBetweenPixelsY = pictureSizeY / pixelsInPictureOn1Line;


        i = 0;
        j = 0;

        StartCoroutine(OnDrawPicture());
    }

    IEnumerator OnDrawPicture()
    {
        Game.CanUseInput = false;

        for (j = 0; j <= pixelsInPictureOn1Line; j++)
        {
            screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (j * distanceBetweenPixelsY);

            for (i = 0; i <= pixelsInPictureOn1Line; i++)
            {
                screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (i * distanceBetweenPixelsX);

                Ray ray = mainCamera.ScreenPointToRay(screenPos);

                List<RaycastHit> hit = new List<RaycastHit>();
                hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

                Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

                CheckPixelColour(hit);

                yield return new WaitForSecondsRealtime(timeInBetweenPicturePixelDrawn);
            }
        }

        Game.CanUseInput = true;

        yield break;
    }

    private void CheckPixelColour(List<RaycastHit> hit)
    {
        if (hit.Count != 0)
        {
            hit = hit.OrderBy(hit => hit.distance).ToList();

            RaycastHit raycastHitTrigger = new RaycastHit();
            Pixel._PixelColour pixelColour = Pixel._PixelColour.White;

            //DrawPixel(hit, Pixel._PixelColour.White);

            if (hit.Count == 1)
            {
                DrawPixel(hit[0], pixelColour);
            }

            if (hit.Count >= 2)
            {
                for (int i = 0; i < hit.Count; i++)
                {
                    if (hit[i].transform.CompareTag(PUSHEABLE_OBJECT_TAG))
                    {
                        if (pixelColour == Pixel._PixelColour.White)
                        {
                            pixelColour = Pixel._PixelColour.Blue;
                        }
                        raycastHitTrigger = hit[i];
                        break;
                    }
                    else if (hit[i].transform.CompareTag(BUTTON_TAG))
                    {
                        if (pixelColour == Pixel._PixelColour.White)
                        {
                            pixelColour = Pixel._PixelColour.Magenta;
                        }
                        raycastHitTrigger = hit[i];
                        break;
                    }
                    else if (hit[i].transform.CompareTag(ENEMY_TAG))
                    {
                        if (pixelColour == Pixel._PixelColour.White)
                        {
                            pixelColour = Pixel._PixelColour.Red;
                        }
                    }
                    else if (hit[i].transform.CompareTag(DOOR_TAG))
                    {
                        if (pixelColour == Pixel._PixelColour.White)
                        {
                            pixelColour = Pixel._PixelColour.Gray;
                        }
                        raycastHitTrigger = hit[i];
                        break;
                    }
                    else
                    {
                        raycastHitTrigger = hit[i];
                        break;
                    }
                }
            }

            DrawPixel(raycastHitTrigger, pixelColour);
        }
    }

    private void DrawPixel(RaycastHit hit, Pixel._PixelColour pixelColour)
    {
        Vector3 pos = new Vector3((float)Math.Round(hit.point.x, 1), (float)Math.Round(hit.point.y, 1), (float)Math.Round(hit.point.z, 1));
      
        GameObject go = pixels.SingleOrDefault<GameObject>(g => g.transform.position == pos);

        GameObject pixelPrefab = pixelColour switch
        {
            Pixel._PixelColour.Red => pixelRed,
            Pixel._PixelColour.Blue => pixelBlue,
            Pixel._PixelColour.Magenta => pixelMagenta,
            Pixel._PixelColour.Gray => pixelGray,
            _ => pixelWhite,
        };

        if (go == null)
        {
            GameObject i = Instantiate(pixelPrefab, pos, Quaternion.identity, pixelsParent);
            pixels.Add(i);
        }
        else if (go.GetComponent<Pixel>().PixelColour != pixelColour)
        {
            Destroy(go);
            pixels.Remove(go);
            GameObject i = Instantiate(pixelPrefab, pos, Quaternion.identity, pixelsParent);
            pixels.Add(i);
        }
    }

    private void RemovePixel(GameObject pixel)
    {
        Destroy(pixel);
        pixels.Remove(pixel);
    }
}
