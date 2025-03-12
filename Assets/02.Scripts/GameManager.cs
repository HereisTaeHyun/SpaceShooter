using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public GameObject monster;
    // 생성된 몬스터 저장 리스트
    public List<GameObject> monsterPool = new List<GameObject>();
    // 풀에 생성할 몬스터 최대 갯수
    public int maxMonsters = 10;
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
        // 몬스터 오브젝트 풀 생성
        CreatMonsterPoll();

        // 스폰포인트 받아오기
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;
        foreach(Transform elem in spawnPointGroup)
        {
            points.Add(elem);
        }

        // 반복 생성
        InvokeRepeating(nameof(CreatMonster), 2.0f, creatTime);
    }

    // 몬스터 풀 제작
    private void CreatMonsterPoll()
    {
        // maxMonsters까지 기본 위치에 monster 생성 후 번호를 매기고 비활성화 후 Pool에 저장
        for(int i = 0; i < maxMonsters; i++)
        {
            GameObject createdMonster = Instantiate<GameObject>(monster);
            createdMonster.name = $"Monster : {i:00}";
            createdMonster.SetActive(false);
            monsterPool.Add(createdMonster);
        }
    }
    // 풀에서 몬스터 가져와 반환
    private GameObject GetMonsterInPool()
    {
        // 비활성화된 몬스터가 있으면 반환, 아니면 null
        foreach(GameObject selectedMonster in monsterPool)
        {
            if(selectedMonster.activeSelf == false)
            {
                return selectedMonster;
            }
        }
        return null;
    }
    // 씬에 몬스터 로드
    private void CreatMonster()
    {
        int idx = Random.Range(0, points.Count);
        GameObject selectedMonter = GetMonsterInPool();
        selectedMonter?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        selectedMonter?.SetActive(true);
    }
}
