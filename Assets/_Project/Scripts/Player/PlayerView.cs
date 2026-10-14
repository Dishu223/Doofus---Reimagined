using UnityEngine;
using TileGame.Core;

namespace TileGame.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class PlayerView : MonoBehaviour, IView
    {
        [SerializeField] private Rigidbody _rigidbody;
        private Vector2 _targetInput;
        private float _targetSpeed;

        public Vector3 CurrentPosition => transform.position;

        private void OnValidate()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            EnsurePhysicsSetup();
        }

        private void EnsurePhysicsSetup()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Create a zero-friction PhysicsMaterial so the cube glides across tile edges without snagging!
            Collider col = GetComponent<Collider>();
            if (col != null && col.sharedMaterial == null)
            {
                PhysicsMaterial frictionless = new PhysicsMaterial("FrictionlessPlayer")
                {
                    dynamicFriction = 0f,
                    staticFriction = 0f,
                    bounciness = 0f,
                    frictionCombine = PhysicsMaterialCombine.Minimum,
                    bounceCombine = PhysicsMaterialCombine.Minimum
                };
                col.material = frictionless;
            }
        }

        public void SetHorizontalVelocity(Vector2 moveInput, float speed)
        {
            _targetInput = moveInput;
            _targetSpeed = speed;

            // Apply immediately as well so movement feels instant
            ApplyVelocity();
        }

        private void FixedUpdate()
        {
            // Keep velocity applied during Unity's physics step so tile friction never slows the cube to 0
            ApplyVelocity();
        }

        private void ApplyVelocity()
        {
            if (_rigidbody == null) return;

            if (_rigidbody.IsSleeping() && _targetInput.sqrMagnitude > 0.001f)
            {
                _rigidbody.WakeUp();
            }

            Vector3 currentVel = _rigidbody.linearVelocity;
            _rigidbody.linearVelocity = new Vector3(
                _targetInput.x * _targetSpeed,
                currentVel.y,
                _targetInput.y * _targetSpeed
            );
        }

        public void ResetToPosition(Vector3 position)
        {
            EnsurePhysicsSetup();
            _targetInput = Vector2.zero;
            _targetSpeed = 0f;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(position, Quaternion.identity);
            _rigidbody.WakeUp();
        }
    }
}