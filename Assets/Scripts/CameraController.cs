using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CS.InputSystem
{
    public class CameraController : MonoBehaviour
    {
        private const string HORIZONTAL_AXIS_MOUSE = "Mouse X";
        private const string VERTICAL_AXIS_MOUSE = "Mouse Y";

        [Header("Options")]

        [Tooltip("Use True if you want this script to drive itself")]
        [SerializeField] private bool _selfSufficient = false;
        
        [SerializeField] private bool _invertHorizontal;
        [Tooltip("Sensitivity of horizontal mouse movement for the camera")]
        [Range(0.1f, 2f)]
        [SerializeField] private float _horizontalSensitivity = 1f;

        [SerializeField] private bool _invertVertical = true;
        [Tooltip("Sensitivity of vertical mouse movement for the camera")]
        
        [Range(0.1f, 2f)]
        [SerializeField] private float _verticalSensitivity = 1f;
        
        [Tooltip("Defines min and max limit for vertical camera angle in degrees calculated from zenith/GlobalUp," +
            " 10 deg: camera looking from almost from horizon; 90 deg: camera looking on character from zenith.")]
        [SerializeField] private Vector2 _verticalMinMaxAngle = new Vector2(10f, 70f);

        #region PRIVATE_METHODS

        private void Start()
        {
            if (_selfSufficient)
            {
                Cursor.lockState = CursorLockMode.Locked;

            }
        }

        private void Update()
        {
            if (_selfSufficient)
            {
                RotateCamera();
            }
        }
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Apply rotation from mouse inputs to this transform to simulate 3rd person camera
        /// </summary>
        public void RotateCamera()
        {
            // Update invert coefs in update in case we change value of serialized bool in runtime
            float invertHorizontalValue = _invertHorizontal ? -1 : 1;
            float invertVerticalValue = _invertVertical ? -1 : 1;

            float horizontalValue = _horizontalSensitivity * invertHorizontalValue  * Input.GetAxisRaw(HORIZONTAL_AXIS_MOUSE);

            float verticalAxisValue = Input.GetAxisRaw(VERTICAL_AXIS_MOUSE);
            float upZenithAngle = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);

            // If we are passing MinMax limit of the vertical axis, clamp input values to one side
            if (upZenithAngle < _verticalMinMaxAngle.x)
            {
                verticalAxisValue = Mathf.Clamp(verticalAxisValue, -1, 0);

            }
            if (upZenithAngle > _verticalMinMaxAngle.y)
            {
                verticalAxisValue = Mathf.Clamp01(verticalAxisValue);
            }
            
            float verticalValue = _verticalSensitivity * invertVerticalValue  * verticalAxisValue;

            // Create rotations quaternions for global Up and local Right with modified input values
            Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalValue, Vector3.up);
            Quaternion verticalRotation = Quaternion.AngleAxis(verticalValue, transform.right);

            // Apply mouse rotations around global Up and Local right to current transform. Order is important
            transform.rotation = horizontalRotation * verticalRotation * transform.rotation;
        }
        #endregion
    }
}
