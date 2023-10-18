using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Player : ProjectBehaviour
{
    private const string PUSHEABLE_OBJECT_TAG = "PusheableObject";
    private const string BUTTON_TAG = "Button";
    private const string DOOR_TAG = "Door";
    private const string ENEMY_TAG = "Enemy";
    private const string MOVING_PLATFORM_TAG = "MovingPlatform";

    [SerializeField] private Camera mainCamera;

    private float maxRaycastDistance = 1000f;

    [SerializeField] Transform pixelsParent;

    [SerializeField] GameObject pixelWhite;
    [SerializeField] GameObject pixelBlue;
    [SerializeField] GameObject pixelRed;
    [SerializeField] GameObject pixelMagenta;
    [SerializeField] GameObject pixelGray;
    [SerializeField] GameObject pixelGreen;

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

    [SerializeField] private VolumeProfile volumeProfile;
    private URPGlitch.Runtime.AnalogGlitch.AnalogGlitchVolume analogGlitchVolume;
    private URPGlitch.Runtime.DigitalGlitch.DigitalGlitchVolume digitalGlitchVolume;

    float maxDigitalGlitchValue = 0.1f;

    float maxScanLineJitterValue = 0.4f;
    float maxVerticalJumpValue = 0.09f;
    float maxHorizontalShakeValue = 0.1f;
    float maxColorDriftValue = 0.28f;

    public float MaxHealth = 100f;
    public float Health;
    public float DamageASec = 30f;
    public float HealthRagenASec = 14f;

    [SerializeField] private GameObject redScreenOverlay;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Health -= DamageASec * Time.deltaTime;
        }

        if (collision.transform.CompareTag("DeathZone"))
        {
            Health -= DamageASec * 2.5f * Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            Health -= DamageASec * Time.deltaTime;
        }

        if (other.transform.CompareTag("DeathZone"))
        {
            Health -= DamageASec * 2.5f * Time.deltaTime;
        }
    }

    private void Awake()
    {
        ProjectBehaviour.GameStart();

        Health = MaxHealth;

        if (!volumeProfile.TryGet(out analogGlitchVolume))
        {
            Debug.Log("None found");
        }

        if (!volumeProfile.TryGet(out digitalGlitchVolume))
        {
            Debug.Log("None found");
        }

        digitalGlitchVolume.intensity.Override(0);
        analogGlitchVolume.colorDrift.Override(0);
        analogGlitchVolume.horizontalShake.Override(0);
        analogGlitchVolume.scanLineJitter.Override(0);
        analogGlitchVolume.verticalJump.Override(0);

        //analogGlitchVolume = volumeProfile.components[0];
        //digitalGlitchVolume = volumeProfile.components[1];

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
        if (Health != MaxHealth)
        {
            Health += HealthRagenASec * Time.deltaTime;

            if (Health > MaxHealth) Health = MaxHealth;

            if (Health < -180f) Health = -180;

            if (Health <= 0f && !Game.PlayerDied) PlayerDeath();
        }

        SetPlayerHealthVisual();

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

    private void SetPlayerHealthVisual()
    {
        float b = 1f;

        if (Health < 25)
        {
            b = 1.45f;
        }
        else if (Health < 30)
        {
            b = 1.35f;
        }
        else if (Health < 35)
        {
            b = 1.3f;
        }
        else if (Health < 40)
        {
            b = 1.25f;
        }
        else if (Health < 45)
        {
            b = 1.2f;
        }
        else if (Health < 50)
        {
            b = 1.15f;
        }
        else if (Health < 55)
        {
            b = 1.12f;
        }
        else if (Health < 60)
        {
            b = 1.09f;
        }
        else if (Health < 65)
        {
            b = 1.06f;
        }
        else if (Health < 70)
        {
            b = 1.03f;
        }
        

        float h = MathF.Abs((Health - MaxHealth) * b);

        digitalGlitchVolume.intensity.Override((maxDigitalGlitchValue / MaxHealth) * h);

        analogGlitchVolume.colorDrift.Override((maxColorDriftValue / MaxHealth) * h);

        if (Health > -50)
        {
            analogGlitchVolume.scanLineJitter.Override((maxScanLineJitterValue / MaxHealth) * h);
        }

        if (h >= 65f)
        {
            analogGlitchVolume.verticalJump.Override((maxVerticalJumpValue / MaxHealth) * h);
        }
        else
        {
            analogGlitchVolume.verticalJump.Override(0);
        }

        if (h >= 80f)
        {
            analogGlitchVolume.horizontalShake.Override((maxHorizontalShakeValue / MaxHealth) * h);
        }
        else
        {
            analogGlitchVolume.horizontalShake.Override(0);
        }
    }

    private void PlayerDeath()
    {
        Game.PlayerDied = true;
        Game.CanUseInput = false;
        redScreenOverlay.SetActive(true);
        DamageASec = 40f;
    }

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            if (drawPictureNextFixedUpdate)
            {
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

    float timeInBetweenPicturePixelDrawn = 0.015f;
    int currentXLine = 0;
    int currentYLine = 0;

    private void DrawPicture()
    {
        distanceBetweenPixelsX = pictureSizeX / pixelsInPictureOn1Line;
        distanceBetweenPixelsY = pictureSizeY / pixelsInPictureOn1Line;

        Game.CanUseInput = false;

        if (currentXLine < pixelsInPictureOn1Line)
        {
            screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (currentYLine * distanceBetweenPixelsY);

            screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (currentXLine * distanceBetweenPixelsX);

            Ray ray = mainCamera.ScreenPointToRay(screenPos);

            List<RaycastHit> hit = new List<RaycastHit>();
            hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            CheckPixelColour(hit);

            currentXLine++;
        }
        else if (currentYLine < pixelsInPictureOn1Line - 1)
        {
            currentXLine = 0;
            currentYLine++;

            screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (currentYLine * distanceBetweenPixelsY);

            screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (currentXLine * distanceBetweenPixelsX);

            Ray ray = mainCamera.ScreenPointToRay(screenPos);

            List<RaycastHit> hit = new List<RaycastHit>();
            hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            CheckPixelColour(hit);

            currentXLine++;
        }
        else
        {
            Game.CanUseInput = true;
            drawPictureNextFixedUpdate = false;
            currentXLine = 0;
            currentYLine = 0;
        }

        //StartCoroutine(OnDrawPicture());
    }

    //IEnumerator OnDrawPicture()
    //{
    //    Game.CanUseInput = false;

    //    for (j = 0; j < pixelsInPictureOn1Line; j++)
    //    {
    //        screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (j * distanceBetweenPixelsY);

    //        for (i = 0; i < pixelsInPictureOn1Line; i++)
    //        {
    //            screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (i * distanceBetweenPixelsX);

    //            Ray ray = mainCamera.ScreenPointToRay(screenPos);

    //            List<RaycastHit> hit = new List<RaycastHit>();
    //            hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

    //            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

    //            CheckPixelColour(hit);

    //            yield return new WaitForSecondsRealtime(timeInBetweenPicturePixelDrawn);
    //        }
    //    }

    //    Game.CanUseInput = true;

    //    yield break;
    //}

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
                    else if (hit[i].transform.CompareTag(MOVING_PLATFORM_TAG))
                    {
                        if (pixelColour == Pixel._PixelColour.White)
                        {
                            pixelColour = Pixel._PixelColour.Green;
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

                DrawPixel(raycastHitTrigger, pixelColour);
            }
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
            Pixel._PixelColour.Green => pixelGreen,
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
