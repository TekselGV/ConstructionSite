using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS.InputSystem;

namespace CS.PlayerBehaviour
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("Script references")]
        [SerializeField] private CameraController _cameraController;
        
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
        }

        private void UpdateCursorLockMode()
        {
            CursorLockMode lockMode = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.lockState = lockMode;
        }
        #endregion
    }
}
