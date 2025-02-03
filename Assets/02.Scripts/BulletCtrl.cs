using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f;
    public float force = 1500.0f;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        // rigidbody 연결 후 전진 방향으로 힘 가하기
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(transform.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
