using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Input = UnityEngine.Input;

public class Player : ProjectBehaviour
{
    public static Player Instance;

    private const string PUSHEABLE_OBJECT_TAG = "PusheableObject";
    private const string BUTTON_TAG = "Button";
    private const string DOOR_TAG = "Door";
    private const string HAZARD_TAG = "Hazard";
    private const string GLASS_TAG = "Glass";
    private const string ENEMY_TAG = "Enemy";
    private const string MOVING_PLATFORM_TAG = "MovingPlatform";

    [SerializeField] private Camera mainCamera;

    private float maxRaycastDistance = 1000f;

    [SerializeField] Transform voxelsParent;

    [SerializeField] GameObject voxelPrefab;

    public class VoxelObject
    {
        public GameObject GameObject { get; set; }
        public Pixel Pixel { get; set; }
    }

    public List<GameObject> voxels = new List<GameObject>();
    public List<VoxelObject> Voxels = new List<VoxelObject>();

    public static List<GameObject> flashLightVoxels = new List<GameObject>();

    bool drawPictureNextFixedUpdate = false;
    bool tryGrabObjectNextFixedUpdate = false;
    bool draw1RandomDirLineFixedUpdate = false;
    bool drawDirLineForPixelRemovalFixedUpdate = false;
    bool useFlashLight = false;

    [SerializeField] private LayerMask mapMask;
    [SerializeField] private LayerMask grabeableObjectMask;
    [SerializeField] private LayerMask pixelMask;

    [SerializeField] private ObjectHolder objectHolder;

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

    public StarterAssetsInputs _input;

    [SerializeField] private GameObject redScreenOverlay;
    [SerializeField] private GameObject console;
    [SerializeField] private GameObject pausedMenu;
    public GameObject Lights;

    //private float timeBetweenFlashUpdate = 0.2f;
    //private float timeBetweenFlashUpdateDelta;

    private void OnCollisionStay(Collision collision)
    {
        if (!Game.GamePaused)
        {
            if (collision.transform.CompareTag("Enemy"))
            {
                Health -= DamageASec * Time.deltaTime;
            }

            if (collision.transform.CompareTag("Hazard"))
            {
                Health -= DamageASec * Time.deltaTime;
            }

            if (collision.transform.CompareTag("DeathZone"))
            {
                Health -= DamageASec * 2.5f * Time.deltaTime;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Game.GamePaused)
        {
            if (other.transform.CompareTag("Enemy"))
            {
                Health -= DamageASec * Time.deltaTime;
            }

            if (other.transform.CompareTag("Hazard"))
            {
                Health -= DamageASec * Time.deltaTime;
            }

            if (other.transform.CompareTag("DeathZone"))
            {
                Health -= DamageASec * 2.5f * Time.deltaTime;
            }
        }
    }

    private void Awake()
    {
        //timeBetweenFlashUpdateDelta = timeBetweenFlashUpdate;

        Instance = this;

        ProjectBehaviour.GameStart(false);

        _input = GetComponent<StarterAssetsInputs>();

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
    }

    private void Update()
    {
        if (objectHolder.AttachedRigidbody == null)
        {
            if (lastHitObject.transform != null)
            {
                lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectLetGoOf();
            }

            grabbedSomeThing = false;
            objectHolder.AttachedRigidbody = null;
        }

        if (Health != MaxHealth)
        {
            if (!Game.PlayerDied)
            {
                Health += HealthRagenASec * Time.deltaTime;
            }
            else
            {
                Health -= DamageASec * 0.85f * Time.deltaTime;
            }

            if (Health > MaxHealth) Health = MaxHealth;

            if (Health < -180f) Health = -180;

            if (Health <= 0f && !Game.PlayerDied) PlayerDeath();
        }

        SetPlayerHealthVisual();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            console.SetActive(false);

            pausedMenu.SetActive(!pausedMenu.activeSelf);
            if (pausedMenu.activeSelf)
            {
                _input.cursorInputForLook = false;
                _input.cursorLocked = false;
                PauseGame();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                _input.cursorInputForLook = true;
                _input.cursorLocked = true;
                ResumeGame();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            console.SetActive(!console.activeSelf);
            if (console.activeSelf)
            {
                _input.cursorInputForLook = false;
                _input.cursorLocked = false;
                PauseGame();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                _input.cursorInputForLook = true;
                _input.cursorLocked = true;
                ResumeGame();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
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

            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    useFlashLight = !useFlashLight;
            //}

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

    private void PlayerDeath()
    {
        Game.PlayerDied = true;
        DisableInput();
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

                DrawRaysForVoxelRemoval();
            }

            if (tryGrabObjectNextFixedUpdate)
            {
                tryGrabObjectNextFixedUpdate = false;

                TryGrabObject();
            }

            //RemoveFlashLightVoxels();

            //if (useFlashLight)
            //{
            //    //string g = Time.realtimeSinceStartupAsDouble.ToString();
            //    //Debug.Log("g = " + g);

            //    DrawFlashLightVoxels();

            //    //string d = Time.realtimeSinceStartupAsDouble.ToString();
            //    //Debug.Log("d = " + d);
            //}


            //timeBetweenFlashUpdateDelta += Time.deltaTime;

            //if (timeBetweenFlashUpdateDelta >= timeBetweenFlashUpdate)
            //{
            //    timeBetweenFlashUpdateDelta = 0;

            //    RemoveFlashLightVoxels();

            //    if (useFlashLight)
            //    {
            //        //string g = Time.realtimeSinceStartupAsDouble.ToString();
            //        //Debug.Log("g = " + g);

            //        DrawFlashLightVoxels();

            //        //string d = Time.realtimeSinceStartupAsDouble.ToString();
            //        //Debug.Log("d = " + d);
            //    }
            //}
        }
    }

    //int amountPointsInRing = 0;
    //double radius = 5d * Screen.height / 1080;

    //private void DrawFlashLightVoxels()
    //{
    //    List<Vector2> vectors = DrawCircleWithPoints(amountPointsInRing, radius, new Vector2(Screen.width / 2, Screen.height / 2));

    //    //string g = Time.realtimeSinceStartupAsDouble.ToString();
    //    //Debug.Log("g = " + g);

    //    foreach (Vector2 i in vectors)
    //    {
    //        //string g = Time.realtimeSinceStartupAsDouble.ToString();
    //        //Debug.Log("g = " + g);

    //        Vector3 pos = i;
    //        Ray ray = mainCamera.ScreenPointToRay(pos);

    //        List<RaycastHit> hit = new List<RaycastHit>();
    //        hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();


    //        //string d = Time.realtimeSinceStartupAsDouble.ToString();
    //        //Debug.Log("d = " + d);

    //        //string g = Time.realtimeSinceStartupAsDouble.ToString();
    //        //Debug.Log("g = " + g);

    //        CheckVoxelColour(hit, false);

    //        //string d = Time.realtimeSinceStartupAsDouble.ToString();
    //        //Debug.Log("d = " + d);
    //    }


    //    //string d = Time.realtimeSinceStartupAsDouble.ToString();
    //    //Debug.Log("d = " + d);
    //}

    //private void MoveFlashLightVoxels()
    //{
    //    List<Vector2> vectors = DrawCircleWithPoints(amountPointsInRing, radius, new Vector2(Screen.width / 2, Screen.height / 2));

    //    for (int i = 0; i < flashLightVoxels.Count; i++)
    //    {
    //        Vector3 pos = vectors[i];
    //        Ray ray = mainCamera.ScreenPointToRay(pos);

    //        List<RaycastHit> hit = new List<RaycastHit>();
    //        hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

    //        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);
    //        ReturnPixel rp = CheckVoxelColour(hit, flashLightVoxels[i].GetComponent<Pixel>(), false);
    //        if (rp.b)
    //        {
    //            flashLightVoxels[i].SetActive(false);
    //            flashLightVoxels.Remove(flashLightVoxels[i]);
    //        }
    //        else
    //        {
    //            if (!(rp.pos == Vector3.positiveInfinity))
    //            {
    //                flashLightVoxels[i].transform.position = rp.pos;
    //            }
    //            else
    //            {
    //                flashLightVoxels[i].SetActive(false);
    //                flashLightVoxels.Remove(flashLightVoxels[i]);
    //            }
    //        }
    //    }
    //}

    //private void RemoveFlashLightVoxels()
    //{
    //    List<GameObject> flashLightsVoxels = new List<GameObject>();

    //    foreach (GameObject g in voxels)
    //    {
    //        if (g.GetComponent<Pixel>().Permanent == false && g.activeInHierarchy)
    //        {
    //            flashLightsVoxels.Add(g);
    //        }
    //    }

    //    foreach (GameObject i in flashLightsVoxels)
    //    {
    //        i.SetActive(false);
    //    }
    //}

    //private void RemoveFlashLightVoxels()
    //{
    //    //foreach (var v in voxels.Where(g => g.GetComponent<Pixel>().Permanent == false))
    //    //{
    //    //    v.SetActive(false);
    //    //}

    //    foreach (var v in Voxels.Where(g => g.Pixel.Permanent == false))
    //    {
    //        v.GameObject.SetActive(false);
    //    }

    //    //List<GameObject> flashLightsVoxels = voxels.FindAll(g => g.GetComponent<Pixel>().Permanent == false);

    //    //for (int i = 0; i < flashLightsVoxels.Count; i++)
    //    //{
    //    //    flashLightsVoxels[i].SetActive(false);
    //    //}
    //}

    //private List<Vector2> DrawCircleWithPoints(int pointsInRing, double radius, Vector2 center)
    //{
    //    double d = radius;
    //    radius = 0;

    //    List<Vector2> vectors = new List<Vector2>();

    //    for (int i = 0; i < 10; i++)
    //    {
    //        radius += d * i;
    //        pointsInRing++;

    //        double spaceBetweenPoints = 360 / pointsInRing;

    //        for (int j = 0; j < pointsInRing; j++)
    //        {
    //            double angle = (spaceBetweenPoints * j);

    //            double newX = center.x + radius * Math.Cos(angle * Math.PI / 180);
    //            double newY = center.y + radius * Math.Sin(angle * Math.PI / 180);

    //            vectors.Add(new Vector2((float)newX, (float)newY));
    //        }
    //    }

    //    return vectors;
    //}

    bool grabbedSomeThing = false;

    private void TryGrabObject()
    {
        if (grabbedSomeThing == false)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 2.2f, grabeableObjectMask))
            {
                lastHitObject = hit;
                Debug.DrawLine(mainCamera.transform.position, mainCamera.transform.position + (mainCamera.transform.forward * 3), Color.magenta, 5f);

                lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectHeld();

                grabbedSomeThing = true;
                objectHolder.AttachedRigidbody = hit.rigidbody;
            }
        }
        else
        {
            lastHitObject.transform.gameObject.GetComponent<MoveableObject>().ObjectLetGoOf();

            grabbedSomeThing = false;
            objectHolder.AttachedRigidbody = null;
        }
    }

    int timesDrawRaysForVoxelRemoval = 8;
    private void DrawRaysForVoxelRemoval()
    {
        for (int i = 0; i < timesDrawRaysForVoxelRemoval; i++)
        {
            Physics.Raycast(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.15f, 0.15f), mainCamera.transform.forward.y + Random.Range(-0.15f, 0.15f), mainCamera.transform.forward.z + Random.Range(-0.15f, 0.15f)), out RaycastHit hit, maxRaycastDistance, pixelMask);
            
            if (hit.transform != null && hit.transform.gameObject.layer == 6)
            {
                RemoveVoxel(hit.transform.gameObject);
            }
        }
    }

    private void Draw1RandomDirLine()
    {
        List<RaycastHit> hit = new List<RaycastHit>();
        hit = Physics.RaycastAll(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.2f, 0.2f), mainCamera.transform.forward.y + Random.Range(-0.2f, 0.2f), mainCamera.transform.forward.z + Random.Range(-0.2f, 0.2f)), maxRaycastDistance, mapMask).ToList();

        CheckVoxelColour(hit);

    }

    float pictureSizeX = 800f * Screen.width / 1920;
    float pictureSizeY = 800f * Screen.height / 1080;

    int voxelsInPictureOn1Line = 10;

    float distanceBetweenVoxelsX;
    float distanceBetweenVoxelsY;

    Vector3 screenPos = Vector3.zero;

    int currentXLine = 0;
    int currentYLine = 0;

    private void DrawPicture()
    {
        distanceBetweenVoxelsX = pictureSizeX / voxelsInPictureOn1Line;
        distanceBetweenVoxelsY = pictureSizeY / voxelsInPictureOn1Line;

        DisableInput();
        
        if (currentXLine < voxelsInPictureOn1Line)
        {
            screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (currentYLine * distanceBetweenVoxelsY);

            screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (currentXLine * distanceBetweenVoxelsX);

            Ray ray = mainCamera.ScreenPointToRay(screenPos);

            List<RaycastHit> hit = new List<RaycastHit>();
            hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            CheckVoxelColour(hit);

            currentXLine++;
        }
        else if (currentYLine < voxelsInPictureOn1Line - 1)
        {
            currentXLine = 0;
            currentYLine++;

            screenPos.y = ((Screen.height / 2) - pictureSizeY / 2) + (currentYLine * distanceBetweenVoxelsY);

            screenPos.x = ((Screen.width / 2) - pictureSizeX / 2) + (currentXLine * distanceBetweenVoxelsX);

            Ray ray = mainCamera.ScreenPointToRay(screenPos);

            List<RaycastHit> hit = new List<RaycastHit>();
            hit = Physics.RaycastAll(ray, maxRaycastDistance, mapMask).ToList();

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

            CheckVoxelColour(hit);

            currentXLine++;
        }
        else
        {
            EnableInput();
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

    //private ReturnPixel CheckVoxelColour(List<RaycastHit> hit, Pixel voxel, bool permanent)
    //{
    //    if (hit.Count != 0)
    //    {
    //        hit = hit.OrderBy(hit => hit.distance).ToList();

    //        RaycastHit raycastHitTrigger = new RaycastHit();
    //        Pixel._VoxelColour voxelColour = Pixel._VoxelColour.White;

    //        //DrawPixel(hit, Pixel._PixelColour.White);

    //        if (hit.Count == 1)
    //        {
    //            if (voxel.VoxelColour != voxelColour)
    //            {
    //                DrawVoxel(hit[0], voxelColour, permanent);
    //                return new ReturnPixel(true, new Vector3((float)Math.Round(hit[0].point.x, 1), (float)Math.Round(hit[0].point.y, 1), (float)Math.Round(hit[0].point.z, 1)));
    //            }
    //            else
    //            {
    //                return new ReturnPixel(false, new Vector3((float)Math.Round(hit[0].point.x, 1), (float)Math.Round(hit[0].point.y, 1), (float)Math.Round(hit[0].point.z, 1)));
    //            }
    //        }

    //        if (hit.Count >= 2)
    //        {
    //            for (int i = 0; i < hit.Count; i++)
    //            {
    //                if (hit[i].transform.CompareTag(PUSHEABLE_OBJECT_TAG))
    //                {
    //                    if (voxelColour == Pixel._VoxelColour.White)
    //                    {
    //                        voxelColour = Pixel._VoxelColour.Blue;
    //                    }
    //                    raycastHitTrigger = hit[i];
    //                    break;
    //                }
    //                else if (hit[i].transform.CompareTag(BUTTON_TAG))
    //                {
    //                    if (voxelColour == Pixel._VoxelColour.White)
    //                    {
    //                        voxelColour = Pixel._VoxelColour.Magenta;
    //                    }
    //                    raycastHitTrigger = hit[i];
    //                    break;
    //                }
    //                else if (hit[i].transform.CompareTag(ENEMY_TAG))
    //                {
    //                    if (voxelColour == Pixel._VoxelColour.White)
    //                    {
    //                        voxelColour = Pixel._VoxelColour.Red;
    //                    }
    //                }
    //                else if (hit[i].transform.CompareTag(DOOR_TAG))
    //                {
    //                    if (voxelColour == Pixel._VoxelColour.White)
    //                    {
    //                        voxelColour = Pixel._VoxelColour.Gray;
    //                    }
    //                    raycastHitTrigger = hit[i];
    //                    break;
    //                }
    //                else if (hit[i].transform.CompareTag(MOVING_PLATFORM_TAG))
    //                {
    //                    if (voxelColour == Pixel._VoxelColour.White)
    //                    {
    //                        voxelColour = Pixel._VoxelColour.Green;
    //                    }
    //                    raycastHitTrigger = hit[i];
    //                    break;
    //                }
    //                else
    //                {
    //                    raycastHitTrigger = hit[i];
    //                    break;
    //                }
    //            }

    //            if (voxel.VoxelColour != voxelColour)
    //            {
    //                DrawVoxel(raycastHitTrigger, voxelColour, permanent);
    //                return new ReturnPixel(true, new Vector3((float)Math.Round(raycastHitTrigger.point.x, 1), (float)Math.Round(raycastHitTrigger.point.y, 1), (float)Math.Round(raycastHitTrigger.point.z, 1)));
    //            }
    //            else
    //            {
    //                return new ReturnPixel(false, new Vector3((float)Math.Round(raycastHitTrigger.point.x, 1), (float)Math.Round(raycastHitTrigger.point.y, 1), (float)Math.Round(raycastHitTrigger.point.z, 1)));
    //            }
    //        }
    //        else
    //        {
    //            Vector3 v3 = Vector3.negativeInfinity;
    //            return new ReturnPixel(false, v3);
    //        }
    //    }
    //    else
    //    {
    //        Vector3 v3 = Vector3.negativeInfinity;
    //        return new ReturnPixel(false, v3);
    //    }
    //}

    private void CheckVoxelColour(List<RaycastHit> hit)
    {
        if (hit.Count != 0)
        {
            hit = hit.OrderBy(hit => hit.distance).ToList();

            RaycastHit raycastHitTrigger = new RaycastHit();
            Pixel._VoxelColour voxelColour = Pixel._VoxelColour.White;

            //DrawPixel(hit, Pixel._PixelColour.White);

            if (hit.Count == 1)
            {
                DrawVoxel(hit[0], voxelColour);
            }

            if (hit.Count >= 2)
            {
                foreach (RaycastHit i in hit)
                {
                    Transform t = i.transform;

                    if (t.CompareTag(GLASS_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Blue;
                        }
                    }
                    else if (t.CompareTag(HAZARD_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Red;
                        }
                    }
                    else if (t.CompareTag(PUSHEABLE_OBJECT_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Blue;
                        }
                        raycastHitTrigger = i;
                        break;
                    }
                    else if (t.CompareTag(BUTTON_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Magenta;
                        }
                        raycastHitTrigger = i;
                        break;
                    }
                    else if (t.CompareTag(ENEMY_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Red;
                        }
                        raycastHitTrigger = i;
                        break;
                    }
                    else if (t.CompareTag(DOOR_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Gray;
                        }
                        raycastHitTrigger = i;
                        break;
                    }
                    else if (t.CompareTag(MOVING_PLATFORM_TAG))
                    {
                        if (voxelColour == Pixel._VoxelColour.White)
                        {
                            voxelColour = Pixel._VoxelColour.Green;
                        }
                        raycastHitTrigger = i;
                        break;
                    }
                    else
                    {
                        raycastHitTrigger = i;
                        break;
                    }
                }

                DrawVoxel(raycastHitTrigger, voxelColour);
            }
        }
    }

    private void DrawVoxel(RaycastHit hit, Pixel._VoxelColour voxelColour)
    {
        Vector3 pos = new Vector3((float)Math.Round(hit.point.x, 1), (float)Math.Round(hit.point.y, 1), (float)Math.Round(hit.point.z, 1));

        GameObject go = null;
        foreach (var v in Voxels)
        {
            if (v.GameObject.transform.position == pos && v.GameObject.activeInHierarchy)
            {
                go = v.GameObject; 

                break;
            }
        }
        //foreach (GameObject g in voxels)
        //{
        //    if (g.transform.position == pos && g.activeInHierarchy && g.GetComponent<Pixel>().Permanent)
        //    {
        //        go = g;
        //        break;
        //    }
        //}

        //if (!permanent)
        //{
        //    //GameObject voxel = Instantiate(voxelPrefab, pos, Quaternion.identity);

        //    GameObject voxel = ObjectPooler.current.GetPooledObject();

        //    if (voxel == null) { return; }
        //    voxel.transform.position = pos;
        //    voxel.transform.rotation = Quaternion.identity;

        //    voxel.SetActive(true);

        //    var p = voxel.GetComponent<Pixel>();
        //    p.VoxelColour = voxelColour;
        //    p.SetMaterial();
        //    //p.Parent = hit.transform;
        //    //p.Offset = pos - hit.transform.position;
        //    //p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);
        //    p.Permanent = false;

        //    return;
        //}

        if (go == null)
        {
            GameObject voxel = ObjectPooler.current.GetPooledObject();

            if (voxel == null) { return; }
            voxel.transform.position = pos;
            voxel.transform.rotation = Quaternion.identity;

            voxel.SetActive(true);

            var p = voxel.GetComponent<Pixel>();
            p.VoxelColour = voxelColour;
            p.SetMaterial();
            p.Parent = hit.transform;
            p.Offset = pos - hit.transform.position;
            p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);

            //i.transform.localScale = new Vector3(1 / hit.transform.localScale.x, 1 / hit.transform.localScale.y, 1 / hit.transform.localScale.z);

            //var pixel = i.GetComponent<Pixel>();

            //pixel.Parent = hit.transform;
        }
        else if (go.GetComponent<Pixel>().VoxelColour != voxelColour)
        {
            go.SetActive(false);

            GameObject voxel = ObjectPooler.current.GetPooledObject();

            if (voxel == null) { return; }
            voxel.transform.position = pos;
            voxel.transform.rotation = Quaternion.identity;

            voxel.SetActive(true);

            var p = voxel.GetComponent<Pixel>();
            p.VoxelColour = voxelColour;
            p.SetMaterial();
            p.Parent = hit.transform;
            p.Offset = pos - hit.transform.position;
            p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);

            //i.transform.localScale = new Vector3(i.transform.localScale.x / hit.transform.localScale.x, i.transform.localScale.y / hit.transform.localScale.y, i.transform.localScale.z / hit.transform.localScale.z);

            //var pixel = i.GetComponent<Pixel>();

            //pixel.Parent = hit.transform;
        }
    }

    private void RemoveVoxel(GameObject voxel)
    {
        voxel.SetActive(false);
    }

    public void SetPlayerHealthToMax()
    {
        Health = MaxHealth;
    }

    public void KillPlayer()
    {
        Health = -1f;
    }
}

public class ReturnPixel
{
    public bool b;
    public Vector3 pos;

    public ReturnPixel(bool value, Vector3 position)
    {
        b = value; 
        pos = position;
    }
}
