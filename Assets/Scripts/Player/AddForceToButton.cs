using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceToButton : MonoBehaviour
{
    [SerializeField] private LayerMask buttonLayer;

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, buttonLayer);
        Debug.DrawLine(transform.position, transform.position + Vector3.down, Color.magenta,2f);

        if (hit.transform != null)
        {
            hit.transform.gameObject.GetComponent<Button>().AddForceNextFixedUpdate = true;
        }
    }
}
