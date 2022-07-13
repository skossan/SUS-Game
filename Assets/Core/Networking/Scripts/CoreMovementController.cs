using Core.Logging;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Core.Networking.Scripts
{
    [AddComponentMenu("CoreSystem/CoreMovementController")]
    public class CoreMovementController : NetworkBehaviour
    {
        private float _turnSmoothVelocity;
        private Vector3 _movementOffset;

        private CharacterController _characterController;
        private Transform _cameraTransform;

        private float _verticalSpeed;
        
        //Setters / Getters
        public Vector3 MovementOffset => _movementOffset;

        public void RunSetup(CharacterController characterController, Transform cameraTransform, float gravityForce)
        {
            _characterController = characterController;
            _cameraTransform = cameraTransform;
        }

        /// <summary>
        /// This process should only be called in Update
        /// </summary>
        public void MovementProcess(float speed, float smoothTime, float gravityForce)
        {
            // Gravity
            if (_characterController.isGrounded)
            {
                _verticalSpeed = 0;
            }
            else
            {
                _verticalSpeed -= gravityForce * Time.deltaTime;
            }
            
            //Movement starts here
            if (_movementOffset.magnitude < 0.1f)
            {
                // Makes sure gravity is applied always but saves callback if player is grounded 
                if (!_characterController.isGrounded)
                {
                    _characterController.Move(new Vector3(0, _verticalSpeed, 0).normalized * (speed * Time.deltaTime));
                }
                return;
            }
        
            // Gets the target Angle relative to the camera rotation
            float targetAngle = Mathf.Atan2(_movementOffset.x, _movementOffset.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        
            // Smooths the rotation of the GameObject
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
            // Gets the new direction that the GameObject should move in
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            moveDirection.y = _verticalSpeed;
            
            _characterController.Move(new Vector3(moveDirection.x, moveDirection.y, moveDirection.z).normalized * (speed * Time.deltaTime));
        }

        // Receives messages from the Input system SendMessage OnMove
        [Client(RequireOwnership = true)]
        public void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector3>();

            _movementOffset = value;
        }
    }
}
