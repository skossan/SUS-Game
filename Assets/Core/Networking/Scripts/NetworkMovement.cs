using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Networking.Scripts
{
    [AddComponentMenu("CoreSystem/NetworkMovement")]
    public class NetworkMovement : NetworkBehaviour
    {

        private float _speed;
        private float _smoothTime;
        private float _turnSmoothVelocity;
        private Vector3 _movementOffset;

        private CharacterController _characterController;
        private Transform _camera;

        //Setters allows 
        public Transform PlayerCamera
        {
            set => _camera = value;
        }

        public CharacterController CharacterController
        {
            set => _characterController = value;
        }

        public float Speed
        {
            set => _speed = value;
        }

        public float SmoothTime
        {
            set => _smoothTime = value;
        }

        public void SetOwner(NetworkConnection newOwner)
        {
            GiveOwnership(newOwner);
        }

        private void Update()
        {
            // Networking, make sure only the owner will be using the Update of this GameObject
            if (!IsOwner)
            {
                return;
            }
        
            MovementProcess(_movementOffset, _speed);
        }

        private void MovementProcess(Vector3 offset, float speed)
        {
        
            if (offset.magnitude < 0.1f)
            {
                return;
            }
        
            // Gets the target Angle relative to the camera rotation
            float targetAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        
            // Smooths the rotation of the GameObject
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
            // Gets the new direction that the GameObject should move in
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            _characterController.Move(moveDirection * (speed * Time.deltaTime));
        }
    
        // Recieves messages from the Input system SendMessage OnMove
        public void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector3>();

            _movementOffset = value;
        }
    }
}
