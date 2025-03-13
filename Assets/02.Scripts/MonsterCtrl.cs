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

    #region public
    public State state = State.IDLE;
    public float traceDistance = 10.0f;
    public float attackDistance = 2f;
    public bool isDie = false;
    #endregion

    #region private
    private const int MAX_HP = 100;
    private const int DAMAGE = 10;
    private const int KILLSCORE = 50;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator animator;
    private int hp = MAX_HP;

    // 혈흔 효과 프리펩
    private GameObject bloodEffect;
    #endregion

    #region Hash
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    #endregion
    // Start is called before the first frame update

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // MonsterTr, PlayerTr을 agent의 destination에 할당
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();

        // 피격시 보일 이펙트
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }

    // OnEnable, OnDisable에서 이벤트 함수 연결 및 해제
    void OnEnable()
    {
        // OnPlayerDie 이벤트에 함수 등록
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
        // 몬스터 상태 체크 코루틴 호출
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void OnDisable()
    {
        // OnPlayerDie 이벤트에 함수 제거
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private IEnumerator CheckMonsterState()
    {
        while(isDie != true)
        {
            yield return new WaitForSeconds(0.3f);

            if(state == State.DIE)
            {
                yield break;
            }

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
                    isDie = true;
                    agent.isStopped = true;
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;

                    // 주먹 콜라이더를 찾아 제거
                    SphereCollider[] Fists = GetComponentsInChildren<SphereCollider>();
                    foreach(var elem in Fists)
                    {
                        elem.enabled = false;
                    }

                    // 일정 시간 대기, 초기 상태로 원복 후 pool로 되돌리기
                    yield return new WaitForSeconds(3.0f);
                    hp = MAX_HP;
                    isDie = false;
                    state = State.IDLE;
                    GetComponent<CapsuleCollider>().enabled = true;
                    foreach(var elem in Fists)
                    {
                        elem.enabled = true;
                    }
                    this.gameObject.SetActive(false);
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

            hp -= DAMAGE;
            if(hp <= 0)
            {
                state = State.DIE;
                GameManager.instance.DisplayScore(KILLSCORE);
            }
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
