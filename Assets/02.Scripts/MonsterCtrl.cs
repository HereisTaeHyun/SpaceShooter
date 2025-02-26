using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE,
    }
    public State state = State.IDLE;
    public float traceDistance = 10.0f;
    public float attackDistance = 2f;
    public bool isDie = false;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator animator;

    // 혈흔 효과 프리펩
    private GameObject bloodEffect;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    // Start is called before the first frame update
    void Start()
    {
        // MonsterTr, PlayerTr을 agent의 destination에 할당
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        // 몬스터 상태 체크 코루틴 호출
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // OnEnable, OnDisable에서 이벤트 함수 연결 및 해제
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private IEnumerator CheckMonsterState()
    {
        while(isDie != true)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);
            if(distance <= attackDistance)
            {
                state = State.ATTACK;
            }
            else if(distance <= traceDistance)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if(state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDistance);
        }

        if(state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }

    private IEnumerator MonsterAction()
    {
        while(isDie != true)
        {
            switch(state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    animator.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    animator.SetBool(hashTrace, true);
                    animator.SetBool(hashAttack, false);
                    agent.isStopped = false;
                    break;
                case State.ATTACK:
                    animator.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 총에 맞으면 효과 생성
        if(collision.collider.CompareTag("BULLET"))
        {
            Destroy(collision.gameObject);
            animator.SetTrigger(hashHit);

            // 총알의 충돌 지점에 혈흔 효과 생성
            Vector3 pos = collision.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            ShowBloodEffect(pos, rot);
        }
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        // bloodEffect를 pos, rot에 monsterTr 자식으로 붙임
        // 특정 tranform의 자식으로 붙이는 방식은 매우 유효, 하이라키 정리 및 같이 이동하기 편함
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        animator.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        animator.SetTrigger(hashPlayerDie);
    }
}
