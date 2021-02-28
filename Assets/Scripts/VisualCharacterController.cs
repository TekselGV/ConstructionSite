using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CS.PlayerBehaviour
{
    [RequireComponent(typeof(Animator))]
    public class VisualCharacterController : MonoBehaviour
    {
        private const string VELOCITY_ANIM_PARAM = "Velocity";

        [Header("Main References")]
        [SerializeField] private Animator _animator;

        [Header("Ragdoll")]
        [SerializeField] private Rigidbody[] _ragdollRigidbodies;
        [SerializeField] private Collider[] _ragdollColliders;

        #region PUBLIC_METHODS
        /// <summary>
        /// Updated character controller animator velocity param with actual velocity from rigidbody
        /// </summary>
        public void UpdateAnimatorVelocity(float velocity)
        {
            _animator.SetFloat(VELOCITY_ANIM_PARAM, velocity);
        }

        /// <summary>
        /// Trigger's visual character to enter ragdoll state i.e. 
        /// disabling animator and enabling radgoll simulations
        /// </summary>
        /// <param name="deathMomentVelocity">Optional velocity value that will be adde to each ragdoll rigidbody</param>
        public void SetRagdollOn(Vector3? deathMomentVelocity = null)
        {
            _animator.enabled = false;
            
            for (int i = 0; i < _ragdollRigidbodies.Length; i++)
            {
                _ragdollRigidbodies[i].isKinematic = false;
                
                if (deathMomentVelocity != null)
                {
                    _ragdollRigidbodies[i].velocity = (Vector3)deathMomentVelocity;
                }
            }

            for (int i = 0; i < _ragdollColliders.Length; i++)
            {
                _ragdollColliders[i].enabled = true;
            }
        }

        #endregion
    }
}
