using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float jump = 10.0f;
    private bool IsJumping;
    void Start()
    {
        IsJumping = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //바닥에 있으면 점프를 실행            
            if (!IsJumping)
            {
                //print("점프 가능 !");                
                IsJumping = true;
                rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
            }
            //공중에 떠있는 상태이면 점프하지 못하도록 리턴            
            else
            {
                //print("점프 불가능 !");                
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            IsJumping = false;
        }
    }
}
