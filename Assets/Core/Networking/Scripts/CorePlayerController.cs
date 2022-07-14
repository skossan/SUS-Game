using System;
using Cinemachine;
using Core.Logging;
using FishNet.Object;
using UnityEngine;

namespace Core.Networking.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("CoreSystem/CorePlayerController")]
    public class CorePlayerController : NetworkBehaviour
    {
        
        //Inspector Variables
        [Header("Movement Stats")]
    
        [SerializeField]
        [Tooltip("Value for the player speed")]
        [InspectorName("Movement Speed")]
        private float speed = 50;
    
        [SerializeField]
        [Tooltip("Rotational Speed of the player")]
        [InspectorName("Rotation Time")]
        [Range(0.001f, 1f)]
        private float smoothTime = 0.1f;

        [SerializeField]
        [Tooltip("Gravity force decides how fast one falls. Requires a negative value")]
        [InspectorName("Gravity Force")]
        private float gravityForce;
        
        [Header("Other Core Components")]
        
        [SerializeField]
        [InspectorName("Core Movement Controller")]
        private CoreMovementController coreMovementController;
        //
        [SerializeField]
        [InspectorName("Core Camera Controller")]
        private CoreCameraController coreCameraController;
    
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
        
        // Private fields
        private CinemachineFreeLook _virtualCamera;

        // Getters Setters
        public CinemachineFreeLook GetVirtualCamera => _virtualCamera;
        public Vector2 GetCameraAxisSpeeds => new Vector2(xCameraSpeed, yCameraSpeed);
        
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
        
            //Validates and makes sure the required components and GameObjects are present, also logs any faults
            characterController ??= GetComponent<CharacterController>();
            if (characterController == null)
            {
                CoreLogging.LogMissingComponent<CharacterController>(gameObject);
            }

            if (Camera.main != null)
            {
                _virtualCamera ??= Camera.main.GetComponent<CinemachineFreeLook>();
                if (_virtualCamera == null)
                {
                    CoreLogging.LogError("Can't find a main camera");
                }
            }

            coreMovementController = GetComponent<CoreMovementController>();
            if (coreMovementController == null)
            {
                CoreLogging.LogMissingComponent<CoreMovementController>(gameObject);
            }
            
            coreCameraController = GetComponent<CoreCameraController>();
            if (coreMovementController == null)
            {
                CoreLogging.LogMissingComponent<CoreCameraController>(gameObject);
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }
            
            coreMovementController.MovementProcess(speed, smoothTime, gravityForce);
        }

        private void RunSetup()
        {
            //Adds a FishNet Network Object Component if there isn't already one
            _ = GetComponent<NetworkObject>() ?? gameObject.AddComponent<NetworkObject>();

            coreMovementController ??= GetComponent<CoreMovementController>();
            if (coreMovementController == null)
            {
                CoreLogging.LogMissingComponent<CoreMovementController>(gameObject);
            }

            _virtualCamera = Camera.main.GetComponent<CinemachineFreeLook>();

            // coreCameraController ??= GetComponent<CoreCameraController>();
            // if (coreCameraController)
            // {
            //     CoreLogging.LogMissingComponent<CoreCameraController>(gameObject);
            // }
            CoreLogging.Log($"CharMoveCont: {characterController}");
            coreMovementController.RunSetup(characterController, _virtualCamera.transform, gravityForce);

            //TODO: Setup Character Controller defaults, needs a proper mesh first
        }
    }
}
