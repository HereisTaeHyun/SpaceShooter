using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // public Transform[] points;
    public List<Transform> points = new List<Transform>();

    void Start()
    {
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
        foreach(Transform elem in spawnPointGroup)
        {
            points.Add(elem);
        }
    }
}
