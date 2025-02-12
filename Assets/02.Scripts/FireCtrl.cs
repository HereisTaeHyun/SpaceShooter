using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 반드시 필요한 컴포넌트를 삭제 안되도록 하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;
    public AudioClip fireSfx;

    // 오디오 컴포넌트
    private new AudioSource audio;
    private MeshRenderer muzzleFlash;

    void Start()
    {
        audio = GetComponent<AudioSource>();  

        // muzzleFlash는 firpos의 자식 객체임
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();

        // 시작시에는 총구 화염 안보여야 함
        muzzleFlash.enabled = false; 
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
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
        // muzzleFlash 활성화
        muzzleFlash.enabled = true;
        // 0.2f 동안 메인 루프에 제어권 넘김
        yield return new WaitForSeconds(0.2f);
        muzzleFlash.enabled = false;
    }
}
