using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    [SerializeField] private Material heldMaterial;
    [SerializeField] private Material normalMaterial;

    public void ObjectHeld()
    {
        this.gameObject.GetComponent<Renderer>().material = heldMaterial;
    }

    public void ObjectLetGoOf()
    {
        this.gameObject.GetComponent<Renderer>().material = normalMaterial;
    }
}
