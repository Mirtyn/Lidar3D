using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] private LayerMask buttonLayer;
    [SerializeField] private Rigidbody buttonrb;
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    public bool AddForceNextFixedUpdate = false;
    private Vector3 force = new Vector3(0, -70f, 0);

    private void FixedUpdate()
    {
        if (AddForceNextFixedUpdate)
        {
            AddForceNextFixedUpdate = false;
            buttonrb.AddForce(force * Time.deltaTime, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == buttonLayer)
        {
            OnPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == buttonLayer)
        {
            OnReleased?.Invoke();
        }
    }
}
