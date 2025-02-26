using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;
    private Animation anim;
    private readonly float initHP = 100.0f; // 초기 생명 값
    private const float DAMAGEHP = 10.0f;

    public float currentHP;
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    // Start is called before the first frame update
    IEnumerator Start() //Start는 void 대신 IEnumerator로 사용 가능
    {
        currentHP = initHP;
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float r = Input.GetAxis("Mouse X");

        // h, v 값에 따라 이동
        Vector3 moveDirection = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDirection.normalized * Time.deltaTime * moveSpeed);

        // 마우스 입력 r을 받아 Vector3.up을 기준으로 변환
        tr.Rotate(Vector3.up * r * turnSpeed * Time.deltaTime);

        // 주인공 애니메이션 설정
        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        //CrossFade의 인자는 어떤 애니메이션으로 바꿀지, 변경되는 시간을 의미
        if(v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);
        }
        else if(v <= -.01f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if(h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if(h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else 
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(currentHP >= 0 && coll.CompareTag("PUNCH"))
        {
            currentHP -= DAMAGEHP;
            Debug.Log($"PlayerHP = {currentHP/initHP}");

            if (currentHP <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    private void PlayerDie()
    {
        
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
    }
}
