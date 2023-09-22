using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : ProjectBehaviour
{
    [SerializeField] private Camera mainCamera;

    private float maxRaycastDistance = 1000f;

    [SerializeField] Transform pixelsParent;

    [SerializeField] GameObject pixelWhite;

    private List<GameObject> pixels = new List<GameObject>();

    bool drawPictureNextFixedUpdate = false;
    bool draw1RandomDirLineFixedUpdate = false;

    [SerializeField] private LayerMask mapMask;

    private void Awake()
    {
        ProjectBehaviour.GameStart();
    }

    private void Update()
    {
        CheckForPlayerInput();
        //Debug.Log(Input.mousePosition);
    }

    private void FixedUpdate()
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
    }

    private void CheckForPlayerInput()
    {
        if (Game.CanUseInput)
        {
            if (Input.GetMouseButton(0))
            {
                draw1RandomDirLineFixedUpdate = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                drawPictureNextFixedUpdate = true;
            }
        }
    }

    private void Draw1RandomDirLine()
    {
        Physics.Raycast(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.3f, 0.3f), mainCamera.transform.forward.y + Random.Range(-0.3f, 0.3f), mainCamera.transform.forward.z + Random.Range(-0.3f, 0.3f)), out RaycastHit hit, maxRaycastDistance, mapMask);

        if (!(hit.transform == null))
        {
            DrawPixel(hit, Pixel._PixelColour.White);
        }
    }

    float pictureSizeX = 3.5f / 7 * Screen.width;
    float pictureSizeY = 3.5f / 7 * Screen.height;

    int pixelsInPictureOn1Line = 8;

    float distanceBetweenPixelsX;
    float distanceBetweenPixelsY;

    Vector3 screenPos = Vector3.zero;


    int i = 0;
    int j = 0;

    float timeInBetweenPicturePixelDrawn = 0.00002f;

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

                Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, mapMask);

                Debug.DrawRay(ray.origin, ray.direction, Color.red, 5);

                if (!(hit.transform == null))
                {
                    DrawPixel(hit, Pixel._PixelColour.White);
                }

                yield return new WaitForSeconds(timeInBetweenPicturePixelDrawn);
            }
        }

        Game.CanUseInput = true;

        yield break;
    }

    private void DrawPixel(RaycastHit hit, Pixel._PixelColour pixelColour)
    {
        Vector3 pos = new Vector3((float)Math.Round(hit.point.x, 1), (float)Math.Round(hit.point.y, 1), (float)Math.Round(hit.point.z, 1));
      
        GameObject go = pixels.SingleOrDefault<GameObject>(g => g.transform.position == pos);

        if (go == null)
        {
            GameObject i = Instantiate(pixelWhite, pos, Quaternion.identity, pixelsParent);
            pixels.Add(i);
        }
        else if (go.GetComponent<Pixel>().PixelColour != pixelColour)
        {
            Destroy(go);
            pixels.Remove(go);
            GameObject i = Instantiate(pixelWhite, pos, Quaternion.identity, pixelsParent);
            pixels.Add(i);
        }
    }
}
