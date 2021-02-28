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
        private const string VELOCITY_ANIM_PARAM = "Velocity";

        private const float FINAL_DIRECTION_THRESHOLD = 0.1f;
        /// <summary>
        /// Defines minimal player rigidbody velocity to react with visual character rotation
        /// </summary>
        private const float MIN_VELOCITY_TO_ROTATE = 0.2f;

        [Header("Script references")]
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private Transform _visualParent;
        [SerializeField] private GameObject _characterPrefab;
        
        [Header("Options")]
        [SerializeField] private bool _lockCursor = true;
        [Range(1f, 20f)]
        [SerializeField] private float _playerRotationSpeed = 7f;

        private Animator _currentCharacterAnimator;
        private GameObject _currentCharacter;

        private bool _isAlive = true;
        
        #region PRIVATE_METHODS
        void Start()
        {
            UpdateCursorLockMode();
            SpawnCharacter();
        }

        private void Update()
        {
            _cameraController.RotateCamera();
            SyncCameraControllerPosition();
            TrackMovementInputs();
            TrackRespawnInput();
            TrackRagdollInput();
            RotateVisualParent();
            UpdateAnimatorVelocity();
        }

        private void UpdateCursorLockMode()
        {
            CursorLockMode lockMode = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.lockState = lockMode;
        }

        /// <summary>
        /// Instantiate character prefab inside visual Parent. Called at the beginning and manually from input
        /// </summary>
        private void SpawnCharacter()
        {
            if (_currentCharacter)
            {
                Destroy(_currentCharacter.gameObject);
            }

            _currentCharacter = Instantiate(_characterPrefab, _visualParent);


            if (!_currentCharacter.TryGetComponent(out _currentCharacterAnimator))
            {
                Debug.LogError("Character prefab doesn't have animator component assigned");
            }


            // We reset physical player back to it's original position here when respawning
            _playerMovement.transform.localPosition = Vector3.zero;
            _playerMovement.SetPlayerDead(false);
            _playerMovement.ResetPlayerVelocity();

            _isAlive = true;
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
            if (!_isAlive)
            {
                return;
            }

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

        private void TrackRespawnInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _isAlive = false;
                SpawnCharacter();
            }
        }

        private void TrackRagdollInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RagdollCharacter();
            }
        }

        /// <summary>
        /// To rotate visual character around local vertical axis along with movement direction
        /// </summary>
        private void RotateVisualParent()
        {
            if (_playerMovement.VelocityRigidbody.magnitude > MIN_VELOCITY_TO_ROTATE && _isAlive)
            {
                Vector3 projectedVelocity = Vector3.ProjectOnPlane(_playerMovement.VelocityRigidbody, _playerMovement.transform.up);
                Quaternion targetOrientation = Quaternion.LookRotation(projectedVelocity, _visualParent.up);
                _visualParent.rotation = Quaternion.Slerp(_visualParent.rotation, targetOrientation, _playerRotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Updated character controller animator velocity param with actual velocity from rigidbody
        /// </summary>
        private void UpdateAnimatorVelocity()
        {
            if (_isAlive)
            {
                _currentCharacterAnimator.SetFloat(VELOCITY_ANIM_PARAM, _playerMovement.VelocityRigidbody.magnitude);
            }
        }

        private void RagdollCharacter()
        {
            _isAlive = false;
            _currentCharacterAnimator.enabled = false;
            _playerMovement.SetPlayerDead(true);
        }
        #endregion
    }
}
