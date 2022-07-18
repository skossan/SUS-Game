using System;
using Cinemachine;
using Core.Logging;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Scripts.CorePlayer
{
    [RequireComponent(typeof(CoreInputActionManager))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(CoreMovementController))]
    [RequireComponent(typeof(CoreCameraController))]
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("CoreSystem/CorePlayerController")]
    public class CorePlayerController : NetworkBehaviour
    {
        
        //Inspector Variables
        [Header("Movement Stats")]
    
        [SerializeField]
        [Tooltip("Value for the player speed")]
        [InspectorName("Movement Speed")]
        private float speed = 10;
    
        [SerializeField]
        [Tooltip("How fast the player will rotate")]
        [InspectorName("Rotation Time")]
        [Range(5, 20)]
        private float rotationTime = 4;

        [SerializeField]
        [Tooltip("Gravity force decides how fast one falls.")]
        [InspectorName("Gravity Force")]
        private float gravityForce = Physics.gravity.y;
        
        [SerializeField]
        [Tooltip("Player jump height")]
        [InspectorName("Jump Height")]
        private float jumpForce = 2;
        
        [SerializeField]
        [InspectorName("Fall Multiplier")]
        [Range(1, 10)]
        private float fallMultiplier = 5;
        
        [Header("Other Core Components")]
        
        [SerializeField]
        [InspectorName("Core Movement Controller")]
        private CoreMovementController coreMovementController;
        //
        [SerializeField]
        [InspectorName("Core Camera Controller")]
        private CoreCameraController coreCameraController;

        [SerializeField] [InspectorName("Core Input Action Manager")]
        private CoreInputActionManager coreActionManager;
        
        [Header("Component Targets")]
    
        [SerializeField]
        [InspectorName("CharacterController Component")]
        private CharacterController characterController;

        [Header("Camera fields")] 
        
        [SerializeField]
        [InspectorName("X axis camera panning speed")]
        private float xCameraSpeed;
        
        [SerializeField]
        [InspectorName("Y axis camera panning speed")]
        private float yCameraSpeed;

        [Header("Input Manager")] 
        [SerializeField]
        [InspectorName("Player Input")]
        private PlayerInput playerInput;

        [SerializeField]
        [InspectorName("Player Input Asset")]
        private InputActionAsset inputAsset;
        
        // Private fields
        private CinemachineFreeLook _virtualCamera;

        // Getters Setters
        public CinemachineFreeLook GetVirtualCamera => _virtualCamera;
        public Vector2 GetCameraAxisSpeeds => new (xCameraSpeed, yCameraSpeed);
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if(!IsOwner)
            {
                return;
            }
            
            RunSetup();
        }
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            characterController ??= GetComponent<CharacterController>();

            coreMovementController ??= GetComponent<CoreMovementController>();
            
            coreCameraController ??= GetComponent<CoreCameraController>();

            coreActionManager ??= GetComponent<CoreInputActionManager>();

            playerInput ??= GetComponent<PlayerInput>();

            inputAsset ??= (InputActionAsset)AssetDatabase.LoadAssetAtPath("Assets/Core/InputSystem/InputMappings/CorePlayer.inputactions", typeof(InputActionAsset));
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            coreMovementController.MovementProcess(speed, rotationTime, gravityForce, jumpForce, fallMultiplier);
        }

        private void RunSetup()
        {
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                _virtualCamera = Camera.main.GetComponent<CinemachineFreeLook>();
                if (_virtualCamera == null)
                {
                    CoreLogging.LogMissingComponent<CinemachineFreeLook>(mainCamera.gameObject);
                }
                
                coreActionManager.RunSetup(playerInput, inputAsset);
            
                coreMovementController.RunSetup(characterController, mainCamera.transform, coreActionManager);
            }
            else
            {
                CoreLogging.LogError("No main camera could be found, please add a camera with MainCamera tag");
            }

            //TODO: Setup Character Controller defaults, needs a proper mesh first
        }
    }
}
