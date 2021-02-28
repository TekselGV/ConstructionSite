using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS.InputSystem;

namespace CS.PlayerBehaviour
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private const string HORIZONTAL_AXIS = "Horizontal";
        private const string VERTICAL_AXIS = "Vertical";

        private const float FINAL_DIRECTION_THRESHOLD = 0.1f;
        /// <summary>
        /// Defines minimal player rigidbody velocity to react with visual character rotation
        /// </summary>
        private const float MIN_VELOCITY_TO_ROTATE = 0.2f;

        [Header("Script references")]
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Transform _visualParent;
        
        [Header("Options")]
        [SerializeField] private bool _lockCursor = true;
        [Range(1f, 20f)]
        [SerializeField] private float _playerRotationSpeed = 7f;

        #region PRIVATE_METHODS
        void Start()
        {
            UpdateCursorLockMode();
        }

        private void Update()
        {
            _cameraController.RotateCamera();
            SyncCameraControllerPosition();
            TrackMovementInputs();
            RotateVisualParent();
        }

        private void UpdateCursorLockMode()
        {
            CursorLockMode lockMode = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.lockState = lockMode;
        }

        /// <summary>
        /// Translate physical player position to camera controller parent
        /// </summary>
        private void SyncCameraControllerPosition()
        {
            _cameraController.transform.position = _playerMovement.transform.position;
        }

        private void TrackMovementInputs()
        {
            // Since input values are changing from -1 to 1, we use this magic operation to remap from 0 to 1
            float remappedHorizontalAxis = Input.GetAxisRaw(HORIZONTAL_AXIS) * 0.5f + 0.5f;
            float remappedVerticalAxis = Input.GetAxisRaw(VERTICAL_AXIS) * 0.5f + 0.5f;

            Vector3 horizontalLerpedDirection = Vector3.Lerp(-1 * _cameraController.transform.right, _cameraController.transform.right,
                remappedHorizontalAxis);
            Vector3 verticalLerpedDirection = Vector3.Lerp(-1 * _cameraController.transform.forward, _cameraController.transform.forward,
                remappedVerticalAxis);

            Vector3 finalDirection = horizontalLerpedDirection + verticalLerpedDirection;

            // To avoid calling extra math calculations, we first check if the player even having some input currenctly
            if (finalDirection.magnitude > FINAL_DIRECTION_THRESHOLD)
            {
                _playerMovement.IsAddingInputForce = true;
                _playerMovement.InputForceVector = Vector3.ProjectOnPlane(finalDirection, Vector3.up);
            }
            else
            {
                _playerMovement.IsAddingInputForce = false;

            }
        }

        /// <summary>
        /// To rotate visual character around local vertical axis along with movement direction
        /// </summary>
        private void RotateVisualParent()
        {
            if (_playerMovement.VelocityRigidbody.magnitude > MIN_VELOCITY_TO_ROTATE)
            {
                Vector3 projectedVelocity = Vector3.ProjectOnPlane(_playerMovement.VelocityRigidbody, _playerMovement.transform.up);
                Quaternion targetOrientation = Quaternion.LookRotation(projectedVelocity, _visualParent.up);
                _visualParent.rotation = Quaternion.Slerp(_visualParent.rotation, targetOrientation, _playerRotationSpeed * Time.deltaTime);
            }
        }
        #endregion
    }
}
