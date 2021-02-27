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
        [SerializeField] private float _horizontalSensitivity = 1f;

        [SerializeField] private bool _invertVertical;
        [Tooltip("Sensitivity of vertical mouse movement for the camera")]
        [Range(0.1f, 2f)]
        [SerializeField] private float _verticalSensitivity = 1f;

        void Update()
        {
            float invertHorizontalValue = _invertHorizontal ? -1 : 1;
            float invertVerticalValue = _invertVertical ? -1 : 1;
            
            float horizontalValue = _horizontalSensitivity * invertHorizontalValue * Input.GetAxisRaw(HORIZONTAL_AXIS_MOUSE);
            
            float verticalValue = _verticalSensitivity * invertVerticalValue * Input.GetAxisRaw(VERTICAL_AXIS_MOUSE);

            Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalValue, Vector3.up);
            Quaternion verticalRotation = Quaternion.AngleAxis(verticalValue, transform.right);
            
            Quaternion rotation = Quaternion.Euler(verticalValue, horizontalValue, 0);

            transform.Rotate(verticalValue, 0f, 0f);

            //transform.rotation = transform.rotation * horizontalRotation * verticalRotation;

        }
    }
}
