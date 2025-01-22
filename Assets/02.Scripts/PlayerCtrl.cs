using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        Vector3 moveDirction = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDirction.normalized * Time.deltaTime * moveSpeed);

        // Vector3 moveDirction = new Vector3(h, 0, v);
        // tr.Translate(moveDirction.normalized * Time.deltaTime * moveSpeed);

        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);
    }
}
