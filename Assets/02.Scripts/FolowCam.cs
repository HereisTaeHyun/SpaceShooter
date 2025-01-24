using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolowCam : MonoBehaviour
{
    public Transform targetTransform;
    private Transform cameraTransform;
    [Range(2f, 20f)]
    public float distance = 10f;
    [Range(0f, 10f)]
    public float height = 2f;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 카메파 pos를 target의 distance 뒤, height 위에 배치
        cameraTransform.position = targetTransform.position + (-targetTransform.forward * distance) + (Vector3.up * height);
        cameraTransform.LookAt(targetTransform.position);
    }
}
