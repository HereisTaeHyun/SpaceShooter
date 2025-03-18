using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 반드시 필요한 컴포넌트를 삭제 안되도록 하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public const float MAXRANGE = 40.0f;
    public GameObject bullet;
    public Transform firePos;
    public AudioClip fireSfx;

    private bool isPlayerDie;
    // 오디오 컴포넌트
    private new AudioSource audio;
    private MeshRenderer muzzleFlash;
    private RaycastHit raycastHit;

    void Start()
    {
        audio = GetComponent<AudioSource>();  

        // muzzleFlash는 firpos의 자식 객체임
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();

        // 시작시에는 총구 화염 안보여야 함
        muzzleFlash.enabled = false; 

        isPlayerDie = false;
    }
    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }
    public void OnPlayerDie() // 주인공이 사망시 공격 기능 비활성화 위한 메서드
    {
        isPlayerDie = true;
    }

    void Update()
    {
        if(isPlayerDie == true)
        {
            return;
        }

        Debug.DrawRay(firePos.position, firePos.forward * MAXRANGE, Color.green);

        if(Input.GetMouseButtonDown(0))
        {
            Fire();
            if(Physics.Raycast(firePos.position, firePos.forward, out raycastHit, MAXRANGE, 1 << LayerMask.NameToLayer("MONSTERBODY")))
            {
                raycastHit.transform.GetComponent<MonsterCtrl>().OnDamage(raycastHit.point, raycastHit.normal);
            }
        }
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
        audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        // offset 좌표값 랜덤 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        // 각도 조절
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(angle, angle, angle);

        // 크기 조절
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // muzzleFlash 활성화
        muzzleFlash.enabled = true;
        // 0.2f 동안 메인 루프에 제어권 넘김
        yield return new WaitForSeconds(0.2f);
        muzzleFlash.enabled = false;
    }
}
