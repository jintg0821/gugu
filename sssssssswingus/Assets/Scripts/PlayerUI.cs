using UnityEngine;
using UnityEngine.UI;

namespace Com.Molinadejan.TestGame
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Text playerNameText;

        [SerializeField]
        private Slider playerHealthSlider;

        private PlayerManager target;

        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        private float characterControllerHeight = 0f;
        private Transform targetTransform;
        private Vector3 targetPosition;

        private void Awake()
        {
            transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        }

        private void Update()
        {
            if(playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }

            if(target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void LateUpdate()
        {
            if(targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }

        public void SetTarget(PlayerManager _target)
        {
            if(_target == null)
            {
                Debug.LogError("Missing PlayerManager target for PlayerUI");
                return;
            }

            target = _target;
            targetTransform = target.transform;

            CharacterController cc = _target.GetComponent<CharacterController>();

            if(cc != null)
            {
                characterControllerHeight = cc.height;
            }

            if(playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }
    }
}
