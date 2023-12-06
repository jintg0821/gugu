using UnityEngine;

namespace Com.Molinadejan.TestGame
{
    public class CameraWork : MonoBehaviour
    {
        [SerializeField]
        private float distance = 7.0f;

        [SerializeField]
        private float height = 3.0f;

        [SerializeField]
        private float heightSmoothLag = 0.3f;

        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [SerializeField]
        private bool followOnStart = false;

        private Transform cameraTransform;

        bool isFollowing;

        private float heightVelocity;

        private float targetHeight = 100000.0f;

        private void Start()
        {
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }

        private void LateUpdate()
        {
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }

            if (isFollowing)
            {
                Apply();
            }
        }

        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            Cut();
        }

        private void Apply()
        {
            Vector3 targetCenter = transform.position + centerOffset;

            float originalTargetAngle = transform.eulerAngles.y;
            float currentAngle = cameraTransform.eulerAngles.y;
            float targetAngle = originalTargetAngle;

            currentAngle = targetAngle;
            targetHeight = targetCenter.y + height;

            float currentHeight = cameraTransform.position.y;
            currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);

            Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);

            cameraTransform.position = targetCenter;
            cameraTransform.position += currentRotation * Vector3.back * distance;

            cameraTransform.position = new Vector3(cameraTransform.position.x, currentHeight, cameraTransform.position.z);

            SetUpRotation(targetCenter);
        }

        private void Cut()
        {
            float oldHeightSmooth = heightSmoothLag;
            heightSmoothLag = 0.001f;
            Apply();
            heightSmoothLag = oldHeightSmooth;
        }

        private void SetUpRotation(Vector3 centerPos)
        {
            Vector3 cameraPos = cameraTransform.position;
            Vector3 offsetToCenter = centerPos - cameraPos;

            Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
            Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
            cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
        }
    }
}
