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

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    // Start is called before the first frame update
    void Start()
    {
        // MonsterTr, PlayerTr을 agent의 destination에 할당
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 몬스터 상태 체크 코루틴 호출
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    // Update is called once per frame
    void Update()
    {
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
        if(collision.collider.CompareTag("BULLET"))
        {
            Destroy(collision.gameObject);
            animator.SetTrigger(hashHit);
        }
    }
}
