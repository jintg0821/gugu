using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Molinadejan.TestGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float speed = 10.0f;
        public float rotateSpeed = 10.0f;
        
        public float Health = 1f;
        float h, v;
        private Rigidbody rb;

        [SerializeField]
        private GameObject beams;

        private bool isFiring;

        [SerializeField]
        private GameObject playerUIPrefab;

        public static GameObject LocalPlayerInstance;

        private void Awake()
        {
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            CameraWork cameraWork = gameObject.GetComponent<CameraWork>();

            if (cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.");
            }

            SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                CalledOnLevelWasLoaded(scene.buildIndex);
            };

            if (playerUIPrefab != null)
            {
                GameObject obj = Instantiate(playerUIPrefab);
                obj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogError("Missing PlayerUIPrefab reference on player prefab");
            }
        }

        private void CalledOnLevelWasLoaded(int level)
        {
            if (this == null)
                return;

            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject obj = Instantiate(playerUIPrefab);
            obj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInput();
            }

            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }

            if (beams != null && isFiring != beams.activeInHierarchy)
            {
                beams.SetActive(isFiring);
            }
        }

        private void ProcessInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            Vector3 dir = new Vector3(h, 0, v); // new Vector3(h, 0, v)가 자주 쓰이게 되었으므로 dir이라는 변수에 넣고 향후 편하게 사용할 수 있게 함

            //바라보는 방향으로 회전 후 다시 정면을 바라보는 현상을 막기 위해 설정
            if (!(h == 0 && v == 0))
            {
                // 이동과 회전을 함께 처리
                transform.position += dir * Time.deltaTime;
                // 회전하는 부분. Point 1.
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            }

        }


        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;

            if (!other.name.Contains("Beam"))
                return;

            Health -= 0.1f;   
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
                return;

            if (!other.name.Contains("Beams"))
                return;

            Health -= 0.1f * Time.deltaTime;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isFiring);
                stream.SendNext(Health);
            }
            else
            {
                isFiring = (bool)stream.ReceiveNext();
                Health = (float)stream.ReceiveNext();
            }
        }
    }
}
