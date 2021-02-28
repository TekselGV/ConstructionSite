using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CS.PlayerBehaviour 
{ 
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        private enum OrientationApproach
        {
            CenterOfMass,
            Constraints
        }
        
        [Header("Main References")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Options")]

        [Tooltip("What method of character orientation use;" +
            "Center of mass - modify center of mass withoffset value trying to keep head up, more dynamic physical approach; " +
            "Constraints - classic braindead way to constraint x/z rotation in rigidbody to always keep character vertical")]
        [SerializeField] private OrientationApproach _orientationApproach = OrientationApproach.CenterOfMass;

        [Tooltip("Vertical offset of player's capsule Center Of Mass to stabilize it's orientation on the world" +
            "so he always tries to stay vertically. 0 - default center of mass position in the pivot point, " +
            "bigger value - better stabilization i.e. move to the local bottom")]
        [Range(0f,5f)]
        [SerializeField] private float _verticalCOMoffset = 3f;

        [Tooltip("How strong we push our character's rigidbody when applying input force")]
        [Range(0f,10000f)]
        [SerializeField] private float _inputForcePower = 100f;

        [Tooltip("Define max player velocity in units/sec")]
        [Range(0f, 14f)]
        [SerializeField] private float _maxPlayerVelocity = 7f;

        /// <summary>
        /// Call to start adding input force
        /// </summary>
        public bool IsAddingInputForce { get; set; }
        /// <summary>
        /// Input force direction vector in world space coordinates
        /// </summary>
        public Vector3 InputForceVector { get; set; }

        public Vector3 VelocityRigidbody => _rigidbody.velocity;

        #region PRIVATE METHODS
        private void Start()
        {
            UpdateOrientationApproach();
        }

        private void FixedUpdate()
        {
            CheckToAddForce();
        }

        private void UpdateOrientationApproach()
        {
            if (_orientationApproach == OrientationApproach.CenterOfMass)
            {
                UpdateCenterOfMass();
                SetRigidbodyConstrains(false);

            }
            else
            {
                SetRigidbodyConstrains(true);
                ResetRigidbodyCenterOfMass();
            }
        }

        /// <summary>
        /// Offsets center of mass along vertical axis to stabilize player orientation
        /// </summary>
        private void UpdateCenterOfMass()
        {
            _rigidbody.centerOfMass = new Vector3(0f, -1 * _verticalCOMoffset, 0f);
        }

        /// <summary>
        /// Resets rigidbody center of mass to default position
        /// </summary>
        private void ResetRigidbodyCenterOfMass()
        {
            _rigidbody.ResetCenterOfMass();
        }

        /// <summary>
        /// Turn's on or off all ridigbody constraints
        /// </summary>
        /// <param name="isConstrained">Set's whether constrains should be on or off</param>
        private void SetRigidbodyConstrains(bool isConstrained)
        {
            if (isConstrained)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            }
            else
            {
                _rigidbody.constraints = RigidbodyConstraints.None;
            }
        }

        private void CheckToAddForce()
        {
            if (IsAddingInputForce && _rigidbody.velocity.magnitude < _maxPlayerVelocity)
            {
                _rigidbody.AddForceAtPosition(InputForceVector.normalized * _inputForcePower, transform.position, ForceMode.Force);
            }
        }
        #endregion

        #region PUBLIC_METHODS

        #endregion
    }
}