using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace Com.Molinadejan.TestGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        private Animator animator;

        [SerializeField]
        private float directionDampTime = 0.25f;

        private float h, v;
        
        private void Start()
        {
            animator = GetComponent<Animator>();

            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
            //else StartCoroutine(GetRandomInput());
        }
        
        private void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
                return;

            if (!animator)
                return;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if(stateInfo.IsName("Base Layer.Run"))
            {
                if(Input.GetButtonDown("Jump"))
                {
                    animator.SetTrigger("Jump");
                }
            }

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            if (v < 0)
                v = 0;

            animator.SetFloat("Speed", h * h + v * v);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }

        IEnumerator GetRandomInput()
        {
            while(true)
            {
                v = Random.Range(0, 2);

                if (v == 0)
                    h = Random.Range(-1, 2);
                else
                    h = 0;

                yield return new WaitForSeconds(6f);
            }
        }
    }
}
