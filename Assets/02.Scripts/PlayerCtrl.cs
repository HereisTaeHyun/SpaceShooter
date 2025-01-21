using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private Transform tr;
    public float moveSpeed = 10f;
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
        Debug.Log($"h = {h}");
        Debug.Log($"v = {v}");

        Vector3 moveDirction = new Vector3(h, 0, v);
        tr.Translate(moveDirction * Time.deltaTime * moveSpeed);

    }
}
