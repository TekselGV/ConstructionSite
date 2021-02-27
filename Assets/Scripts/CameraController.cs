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

        [SerializeField] private bool _invertHorizontal;
        [Tooltip("Sensitivity of horizontal mouse movement for the camera")]
        [Range(0.1f, 2f)]
        [SerializeField] private float _horizontalSensitivity = 150f;

        [SerializeField] private bool _invertVertical = true;
        [Tooltip("Sensitivity of vertical mouse movement for the camera")]
        [Range(0.1f, 2f)]
        [SerializeField] private float _verticalSensitivity = 150f;

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
            float verticalValue = _verticalSensitivity * invertVerticalValue  * Input.GetAxisRaw(VERTICAL_AXIS_MOUSE);

            // Create rotations quaternions for global Up and local Right with modified input values
            Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalValue, Vector3.up);
            Quaternion verticalRotation = Quaternion.AngleAxis(verticalValue, transform.right);

            // Apply mouse rotations around global Up and Local right to current transform. Order is important
            transform.rotation = horizontalRotation * verticalRotation * transform.rotation;
        }
        #endregion
    }
}
