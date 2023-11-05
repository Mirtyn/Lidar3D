using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : ProjectBehaviour
{
    public static ObjectPooler current;

    [SerializeField] private GameObject pooledObject;
    [SerializeField] private float pooledAmount;
    [SerializeField] private bool willGrow;

    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            Player.Instance.voxels.Add(obj);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = Instantiate(pooledObject);
            pooledObjects.Add(obj);
            Player.Instance.voxels.Add(obj);
            return obj;
        }

        return null;
    }
}
