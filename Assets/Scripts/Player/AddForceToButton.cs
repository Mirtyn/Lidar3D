using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceToButton : MonoBehaviour
{
    [SerializeField] private LayerMask buttonLayer;
    private float distanceOfRay = 0.6f;

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distanceOfRay, buttonLayer);
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * distanceOfRay), Color.magenta,2f);

        if (hit.transform != null)
        {
            hit.transform.gameObject.GetComponent<ButtonBase>().AddForceNextFixedUpdate = true;
        }
    }
}
