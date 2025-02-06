using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;
    // 충돌 시작시 발생하는 이벤트

    private void OnCollisionEnter(Collision coll)
    {
        // tag 비교
        if(coll.collider.CompareTag("BULLET"))
        {
            // 충돌 지점 정보 추출
            ContactPoint cp = coll.GetContact(0);
            Quaternion rotate = Quaternion.LookRotation(-cp.normal);
            // 충돌 대상체 스파크 동적 생성 후 삭제
            GameObject spark = Instantiate(sparkEffect, cp.point, rotate);
            Destroy(spark, 0.5f);
            Destroy(coll.gameObject);
        }
    }

}
