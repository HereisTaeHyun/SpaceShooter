using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // 상수 변수
    const int MAX_HIT = 3; // 몇 번 맞아야 폭파하는지
    const float DELETE_TIME_EFFECT = 2.0f;
    const float DELETE_TIME_BARREL = 3.0f;
    const float MASS_BARREL = 1.0f;
    const float FORCE = 1500.0f;

    public Texture[] textures; // 무작위로 적용할 텍스쳐 배열
    public GameObject explosionEffect;
    public float radius = 10.0f;

    private new MeshRenderer renderer; // 자식 오브젝트의 meshRenderer 지정
    private Transform tf;
    private Rigidbody rb;
    private int hitCount = 0;
    private Collider[] colls = new Collider[10];
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponentInChildren<MeshRenderer>();

        // 난수 발생 후 텍스쳐 지정
        int textureIdx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[textureIdx];
    }

    // 충돌 시 발생하는 콜백 함수
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.CompareTag("BULLET"))
        {
            hitCount += 1;
            if(hitCount == MAX_HIT)
            {
                Explosion();
            }
        }
    }

    // 폭파 함수
    void Explosion()
    {
        // 폭파 효과 생성 후 5초 후 오브젝트 제거
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, DELETE_TIME_EFFECT);

        // gameObject 무게를 가볍게 하고 Addforce로 날린 후 제거
        // rb.mass = MASS_BARREL;
        // rb.AddForce(Vector3.up * FORCE);

        // 간접 폭발력 전달
        InDirectDamage(transform.position);
        Destroy(gameObject, DELETE_TIME_BARREL);
    }

    void InDirectDamage(Vector3 pos)
    {
        // 인근 드럼통 추출, 추출 시작 위치, 추출 범위, 레이어로 비트 연산자로 표현
        Physics.OverlapSphereNonAlloc(pos, radius, colls, 1 << 3);
        foreach(var coll in colls)
        {
            // 범위 내 collider의 rb 추출 후 mass 감소 및 축 고정 해제
            // 이후 AddExplosionForce러 원 모양으로 반발력 생성
            if(coll == null) continue;
            rb = coll.GetComponent<Rigidbody>();
            rb.mass = MASS_BARREL;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(FORCE, pos, radius, 1200.0f);
        }
    }
}
