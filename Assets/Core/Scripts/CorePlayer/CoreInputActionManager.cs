using Core.Logging;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Scripts.CorePlayer
{
    /// <summary>
    /// Manager for managing Actions that does not need or want to be event based
    /// </summary>
    public class CoreInputActionManager : NetworkBehaviour
    {
        private Vector3 _inputMovement = new ();
        
        private PlayerInput _playerInput;
        
        private InputAction _jumpAction;

        public InputAction Jump => _jumpAction;
        public Vector3 InputMovement => _inputMovement;

        public void RunSetup(PlayerInput playerInput, InputActionAsset inputActionAsset)
        {
            _playerInput = playerInput;
            CoreLogging.Log($"{inputActionAsset}");
            _playerInput.actions = inputActionAsset;

            EnableActions();
        }

        private void EnableActions()
        {
            _playerInput.actions.Enable();
            
            _jumpAction = _playerInput.actions["Jump"];
            
            _jumpAction.Enable();
        }

        // Receives messages from the Input system SendMessage OnMove
        [Client(RequireOwnership = true)]
        public void OnMove(InputValue inputValue)
        {
            var value = inputValue.Get<Vector3>();

            _inputMovement = value;
        }
    }
}