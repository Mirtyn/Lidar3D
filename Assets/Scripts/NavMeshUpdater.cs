using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdater : ProjectBehaviour
{
    public static NavMeshSurface navMeshSurface;
    public static NavMeshData navMeshData;

    private void OnEnable()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshData = navMeshSurface.navMeshData;
    }

    private void Update()
    {
        if (!Game.GamePaused)
        {
            navMeshSurface.UpdateNavMesh(navMeshData);
        }
    }
}
