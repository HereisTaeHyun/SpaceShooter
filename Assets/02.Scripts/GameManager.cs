using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public GameObject monster;
    public float creatTime = 3.0f;

    private bool isGameOver;

    // 싱글톤 선언
    public static GameManager instance = null;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 게임오버 체크 프로퍼티
    public bool IsGameOver
    {
        get {return isGameOver;}
        set {
            isGameOver = value;
            if(isGameOver == true)
            {
                CancelInvoke(nameof(CreatMonster));
            }
        }
    }

    void Start()
    {
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
        foreach(Transform elem in spawnPointGroup)
        {
            points.Add(elem);
        }

        InvokeRepeating(nameof(CreatMonster), 2.0f, creatTime);
    }

    private void CreatMonster()
    {
        int idx = Random.Range(0, points.Count);
        Instantiate(monster, points[idx].position, points[idx].rotation);
    }
}
