using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuCamera : MonoBehaviour
{
    private GameObject mainCamera;

    [SerializeField] private float rotateSpeed;

    private List<GameObject> pixels = new List<GameObject>();

    [SerializeField] Transform pixelsParent;

    [SerializeField] private GameObject pixelWhite;
    [SerializeField] private GameObject pixelMagenta;
    [SerializeField] private GameObject pixelBlue;
    [SerializeField] private GameObject pixelRed;
    [SerializeField] private GameObject pixelGray;

    //float T;
    //float R;

    private void Awake()
    {
        mainCamera = this.gameObject;
        //T = Random.Range(2.22f, 4.5f);
        //R = Random.Range(0f, 100f);
    }


    private void Update()
    {
        mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y + rotateSpeed * Time.deltaTime, mainCamera.transform.eulerAngles.z);
        
        //T -= Time.deltaTime;

        //if (T <= 0) 
        //{
        //    T = Random.Range(1.22f, 3.21f);
        //    R = Random.Range(0f, 100f);
        //}
    }

    private void FixedUpdate()
    {
        Draw1RandomDirLine();
    }

    private void Draw1RandomDirLine()
    {
        List<RaycastHit> hit = new List<RaycastHit>();
        hit = Physics.RaycastAll(mainCamera.transform.position, new Vector3(mainCamera.transform.forward.x + Random.Range(-0.4f, 0.4f), mainCamera.transform.forward.y + Random.Range(-0.4f, 0.4f), mainCamera.transform.forward.z + Random.Range(-0.4f, 0.4f)), 1000, -1).ToList();

        CheckPixelColour(hit);
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
                    raycastHitTrigger = hit[i];
                    break;
                }
            }

            DrawPixel(raycastHitTrigger, pixelColour);
        }
    }

    private void DrawPixel(RaycastHit hit, Pixel._PixelColour pixelColour)
    {
        Vector3 pos = new Vector3((float)Math.Round(hit.point.x, 1), (float)Math.Round(hit.point.y, 1), (float)Math.Round(hit.point.z, 1));

        GameObject go = pixels.SingleOrDefault<GameObject>(g => g.transform.position == pos);

        int g = Random.Range(0, 500);

        GameObject pixelPrefab;

        if (g >= 5)
        {
            pixelPrefab = pixelWhite;
        }
        else
        {
            pixelPrefab = g switch
            {
                > 3 => pixelWhite,
                > 2 => pixelRed,
                > 1 => pixelMagenta,
                > 0 => pixelBlue,
                _ => pixelGray
            };
        }
        
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
}
