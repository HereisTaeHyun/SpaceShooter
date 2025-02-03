using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 충돌 시작시 발생하는 이벤트

    private void OnCollisionEnter(Collision coll)
    {
        // tag 바교
        if(coll.collider.CompareTag("BULLET"))
        {
            // 충돌 대상체 삭제
            Destroy(coll.gameObject);
        }
    }

}
