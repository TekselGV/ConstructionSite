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

        [Header("Script references")]
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private PlayerMovement _playerMovement;
        
        [Header("Options")]
        [SerializeField] private bool _lockCursor = true;

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

        }
        #endregion
    }
}
