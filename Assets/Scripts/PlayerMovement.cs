using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CS.PlayerBehaviour 
{ 
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Main References")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Options")]
        
        [Tooltip("Vertical offset of player's capsule Center Of Mass to stabilize it's orientation on the world" +
            "so he always tries to stay vertically. 0 - default center of mass position in the pivot point, " +
            "bigger value - better stabilization i.e. move to the local bottom")]
        [Range(0f,3f)]
        [SerializeField] private float _verticalCOMoffset = 1.5f;

        [Tooltip("How strong we push our character's rigidbody when applying input force")]
        [Range(0f,200f)]
        [SerializeField] private float _inputForcePower = 100f;

        /// <summary>
        /// Call to start adding input force
        /// </summary>
        public bool IsAddingInputForce { get; set; }
        /// <summary>
        /// Input force direction vector in world space coordinates
        /// </summary>
        public Vector3 InputForceVector { get; set; }

        #region PRIVATE METHODS
        private void Start()
        {
            UpdateCenterOfMass();
        }

        private void FixedUpdate()
        {
            CheckToAddForce();
        }

        /// <summary>
        /// Offsets center of mass along vertical axis to stabilize player orientation
        /// </summary>
        private void UpdateCenterOfMass()
        {
            _rigidbody.centerOfMass = new Vector3(0f, -1 * _verticalCOMoffset, 0f);
        }

        private void CheckToAddForce()
        {
            if (IsAddingInputForce)
            {
                _rigidbody.AddForce(InputForceVector * _inputForcePower, ForceMode.Force);
            }
        }

        #endregion

        #region PUBLIC_METHODS

        #endregion
    }
}