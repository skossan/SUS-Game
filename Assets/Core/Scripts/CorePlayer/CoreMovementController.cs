using System;
using Core.Logging;
using FishNet.Object;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Scripts.CorePlayer
{
    [AddComponentMenu("CoreSystem/CoreMovementController")]
    public class CoreMovementController : NetworkBehaviour
    {
        private float _turnSmoothVelocity;
        private Vector3 _inputMovement;
        
        // Terminal Velocity for a human being, subject to change
        private float _terminalVelocity = 53;

        private CharacterController _characterController;
        private Transform _cameraTransform;

        private Vector3 _playerVelocity;

        private CoreInputActionManager _coreActionManager;

        public void RunSetup(CharacterController characterController, Transform cameraTransform, CoreInputActionManager coreActionManager)
        {
            _characterController = characterController;
            _cameraTransform = cameraTransform;
            _coreActionManager = coreActionManager;
        }

        /// <summary>
        /// This process should only be called in Update
        /// </summary>
        public void MovementProcess(float speed, float rotationSpeed, float gravityForce, float jumpForce, float fallMultiplier)
        {
            _inputMovement = _coreActionManager.InputMovement;
            var isGrounded = _characterController.isGrounded;
            var gravity = -Math.Abs(gravityForce);
            var terminalVelocity = -Math.Abs(_terminalVelocity);
            
            // Reset falling velocity
            if (isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0;
            }

            // Movement
            Vector3 moveDirection = new Vector3(_inputMovement.x, 0, _inputMovement.z);
            
            // Get the direction of movement relative to the camera transform
            moveDirection = _cameraTransform.forward.normalized * moveDirection.z + _cameraTransform.right.normalized * moveDirection.x;
            _characterController.Move(new Vector3(moveDirection.x, 0, moveDirection.z).normalized * (speed * Time.deltaTime));
            
            // Gravity logic
            _playerVelocity.y += gravity * Time.deltaTime * (fallMultiplier - 1);
            // Makes sure we don't fall faster than terminal velocity
            _playerVelocity.y = _playerVelocity.y < terminalVelocity ? terminalVelocity : _playerVelocity.y;

            // Jump logic
            if (_coreActionManager.Jump.triggered && isGrounded)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
            }
            
            _characterController.Move(_playerVelocity * Time.deltaTime);
            
            // Rotation of character and also makes sure the movement vector stays at 0 if the player lets go of the movement controls
            if(_inputMovement.magnitude < 0.1f)
            {
                _inputMovement = Vector3.zero;
            }
            if(_inputMovement != Vector3.zero)
            {
                var targetAngle = Mathf.Atan2(_inputMovement.x, _inputMovement.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0,targetAngle,0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
