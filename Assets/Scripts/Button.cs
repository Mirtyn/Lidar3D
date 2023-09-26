using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] private LayerMask buttonLayer;
    public Event OnPressed;
    public Event OnReleased;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == buttonLayer)
        {

        }
    }
}
