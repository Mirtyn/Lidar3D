using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Pixel;
using Random = UnityEngine.Random;

public class MainMenuCamera : ProjectBehaviour
{
    private GameObject mainCamera;

    [SerializeField] private float rotateSpeed;

    private List<GameObject> pixels = new List<GameObject>();

    [SerializeField] Transform pixelsParent;

    [SerializeField] private GameObject Voxel;

    private void Awake()
    {
        ProjectBehaviour.GameStart(true);

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
            Pixel._VoxelColour pixelColour = Pixel._VoxelColour.White;

            //DrawPixel(hit, Pixel._PixelColour.White);

            DrawPixel(hit[0], pixelColour);
        }
    }

    private void DrawPixel(RaycastHit hit, Pixel._VoxelColour pixelColour)
    {
        Vector3 pos = new Vector3((float)Math.Round(hit.point.x, 1), (float)Math.Round(hit.point.y, 1), (float)Math.Round(hit.point.z, 1));

        GameObject go = pixels.SingleOrDefault<GameObject>(g => g.transform.position == pos);

        int g = Random.Range(0, 500);

        Pixel._VoxelColour voxelColour;

        voxelColour = g switch
        {
            4 => _VoxelColour.Red,
            3 => _VoxelColour.Blue,
            2 => _VoxelColour.Gray,
            1 => _VoxelColour.Green,
            0 => _VoxelColour.Magenta,
            _ => _VoxelColour.White
        };

        if (go == null)
        {
            GameObject i = Instantiate(Voxel, pos, Quaternion.identity, pixelsParent);

            i.transform.position = pos;
            i.transform.rotation = Quaternion.identity;

            var p = i.GetComponent<Pixel>();
            p.VoxelColour = voxelColour;
            p.SetMaterial();
            p.Parent = hit.transform;
            p.Offset = pos - hit.transform.position;
            p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);

            pixels.Add(i);
            //i.transform.localScale = new Vector3(1 / hit.transform.localScale.x, 1 / hit.transform.localScale.y, 1 / hit.transform.localScale.z);

            //var pixel = i.GetComponent<Pixel>();

            //pixel.Parent = hit.transform;
        }
        else if (go.GetComponent<Pixel>().VoxelColour != voxelColour)
        {
            Destroy(go);
            pixels.Remove(go);

            go.SetActive(false);

            GameObject i = Instantiate(Voxel, pos, Quaternion.identity, pixelsParent);

            i.transform.position = pos;
            i.transform.rotation = Quaternion.identity;

            var p = i.GetComponent<Pixel>();
            p.VoxelColour = voxelColour;
            p.SetMaterial();
            p.Parent = hit.transform;
            p.Offset = pos - hit.transform.position;
            p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);

            pixels.Add(i);
            //i.transform.localScale = new Vector3(i.transform.localScale.x / hit.transform.localScale.x, i.transform.localScale.y / hit.transform.localScale.y, i.transform.localScale.z / hit.transform.localScale.z);

            //var pixel = i.GetComponent<Pixel>();

            //pixel.Parent = hit.transform;
        }

        //if (go == null)
        //{
        //    GameObject i = Instantiate(pixelPrefab, pos, Quaternion.identity, pixelsParent);
        //    var p = i.GetComponent<Pixel>();
        //    p.Parent = hit.transform;
        //    p.Offset = pos - hit.transform.position;
        //    p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);
        //    pixels.Add(i);
        //}
        //else if (go.GetComponent<Pixel>().VoxelColour != pixelColour)
        //{
        //    Destroy(go);
        //    pixels.Remove(go);
        //    GameObject i = Instantiate(pixelPrefab, pos, Quaternion.identity, pixelsParent);
        //    var p = i.GetComponent<Pixel>();
        //    p.Parent = hit.transform;
        //    p.Offset = pos - hit.transform.position;
        //    p.RotOffset = Quaternion.Inverse(Quaternion.identity * hit.transform.rotation);
        //    pixels.Add(i);
        //}
    }
}
