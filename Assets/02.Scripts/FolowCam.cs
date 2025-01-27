using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCam : MonoBehaviour
{
    public Transform targetTransform;
    private Transform cameraTransform;

    [Range(2f, 20f)]
    public float distance = 4f;
    [Range(0f, 10f)]
    public float height = 2f;

    public float damping = 0.1f;
    public float targetOffset = 2f;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 카메파 pos를 target의 distance 뒤, height 위에 배치
        Vector3 pos = targetTransform.position + (-targetTransform.forward * distance) + (Vector3.up * height);

        // 구면 선형 보간으로 위치 변경
        // cameraTransform.position = Vector3.Slerp(cameraTransform.position, pos, Time.deltaTime * damping);
        // smoothdamp로 위치 변경
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, pos, ref velocity, damping);
        cameraTransform.LookAt(targetTransform.position + (targetTransform.up * targetOffset));
    }
}
