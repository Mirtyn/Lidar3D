using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject openDoor;
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private bool isOpened = false;

    [SerializeField] private float moveSpeed = 6;

    void Awake()
    {
        if (closedDoor == null)
        {
            closedDoor = Instantiate(door, door.transform.position, door.transform.rotation);
        }

        closedDoor.SetActive(false);
        openDoor.SetActive(false);
    }

    void Update()
    {
        var target = isOpened ? openDoor : closedDoor;

        door.transform.position = Vector3.MoveTowards(door.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }

    public void OpenDoor()
    {
        isOpened = true;
    }

    public void CloseDoor()
    {
        isOpened = false;
    }
}
