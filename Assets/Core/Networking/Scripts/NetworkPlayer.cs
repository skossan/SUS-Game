using Cinemachine;
using Core.Logging;
using FishNet.Object;
using UnityEngine;

namespace Core.Networking.Scripts
{
    [AddComponentMenu("CoreSystem/NetworkPlayer")]
    public class NetworkPlayer : NetworkBehaviour
    {
        //Inspector Variables
        [Header("Movement Stats")]
    
        [SerializeField]
        [Tooltip("Value for the player speed")]
        [InspectorName("Movement Speed")]
        private float speed = 250;
    
        [SerializeField]
        [Tooltip("Rotational Speed of the player")]
        [InspectorName("Rotation Time")]
        [Range(0.1f, 1f)]
        private float smoothTime = 0.1f;

        [Header("Network Movement")]
    
        [SerializeField]
        [InspectorName("Network Movement Component")]
        private NetworkMovement networkMovement;
    
        [Header("Component Targets")]
    
        [SerializeField]
        [InspectorName("CharacterController Component")]
        private CharacterController characterController;
    
        [Header("Cameras")]
    
        [SerializeField]
        [InspectorName("Player Camera Transform")]
        private Transform playerCamera;

        [SerializeField]
        [InspectorName("Free Look Camera")]
        private CinemachineFreeLook freeLookCamera;

        [Header("Camera Variables")] 
    
        [SerializeField]
        [InspectorName("X Axis Rotation Speed")]
        private float xAxisSpeed = 100f;
    
        [SerializeField]
        [InspectorName("Y Axis Rotation Speed")]
        private float yAxisSpeed = 1.5f;
    
    
        private void Awake()
        {
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

            playerCamera ??= transform.Find("PlayerCamera").transform;
            if (playerCamera == null)
            {
                CoreLogging.LogMissingGameObject<Camera>(gameObject);
            }
        
            freeLookCamera ??= transform.Find("CMFreeLook").gameObject.GetComponent<CinemachineFreeLook>();
            if (freeLookCamera == null)
            {
                CoreLogging.LogMissingComponent<CinemachineFreeLook>(gameObject);
            }

            networkMovement = GetComponent<NetworkMovement>();
            if (networkMovement == null)
            {
                CoreLogging.LogMissingComponent<NetworkMovement>(gameObject);
            }
        }

        private void RunSetup()
        {
            //Adds a FishNet Network Object Component if there isn't already one
            _ = GetComponent<NetworkObject>() ?? gameObject.AddComponent<NetworkObject>();

            ConfigureFreeLookCamera(freeLookCamera);
            ConfigureNetworkMovement(networkMovement);

            //TODO: Setup Character Controller defaults, needs a proper mesh first
        }

        private void ConfigureFreeLookCamera(CinemachineFreeLook cinemachineFreeLookCamera)
        {
            cinemachineFreeLookCamera.m_XAxis.m_MaxSpeed = xAxisSpeed;
            cinemachineFreeLookCamera.m_YAxis.m_MaxSpeed = yAxisSpeed;
        }
        
        private void ConfigureNetworkMovement(NetworkMovement networkMovementComponent)
        {
            networkMovementComponent.PlayerCamera = playerCamera;
            networkMovementComponent.CharacterController = characterController;
            networkMovementComponent.Speed = speed;
            networkMovementComponent.SmoothTime = smoothTime;
        }
    }
}
