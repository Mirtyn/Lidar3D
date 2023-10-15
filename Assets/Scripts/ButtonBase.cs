using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBase : ProjectBehaviour
{
    private LayerMask buttonLayer = 9;
    [SerializeField] private Rigidbody buttonrb;
    [SerializeField] private MapButton mapButton;
    public bool AddForceNextFixedUpdate = false;
    private Vector3 force = new Vector3(0, -55f, 0);

    private void FixedUpdate()
    {
        if (!Game.GamePaused)
        {
            if (AddForceNextFixedUpdate)
            {
                AddForceNextFixedUpdate = false;
                buttonrb.AddForce(force * Time.deltaTime, ForceMode.Force);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Game.GamePaused)
          {
            if (other.gameObject.layer == buttonLayer)
            {
                mapButton.OnPressed?.Invoke();
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Game.GamePaused)
        {
            if (other.gameObject.layer == buttonLayer)
            {
                mapButton.OnReleased?.Invoke();
            }
        }
        
    }
}
