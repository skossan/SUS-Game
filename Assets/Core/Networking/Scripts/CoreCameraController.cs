using System;
using Cinemachine;
using Core.Logging;
using FishNet.Object;
using UnityEngine;

namespace Core.Networking.Scripts
{
    [AddComponentMenu("CoreSystem/CoreCameraController")]
    public class CoreCameraController : NetworkBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                return;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var corePlayerController = GetComponent<CorePlayerController>();
            if (corePlayerController != null)
            {
                var cameraSpeed = corePlayerController.GetCameraAxisSpeeds;
                RunSetup(corePlayerController.GetVirtualCamera, transform, cameraSpeed.x, cameraSpeed.y);
            }
            else
            {
                CoreLogging.LogMissingComponent<CorePlayerController>(gameObject);
            }
        }

        /// <summary>
        /// Will automatically assign a follow target and a look at target by Core conventions. <br/>
        /// This will not override any already existing follow or look at targets on the virtual camera
        /// </summary>
        private void RunSetup(CinemachineFreeLook virtualCamera, Transform parentTransform, float xCameraSpeed, float yCameraSpeed)
        {
            virtualCamera.Follow = parentTransform;
            if (virtualCamera.Follow == null)
            {
                CoreLogging.LogError($"Missing FollowTarget GameObject on {gameObject.name}");
            }
            
            virtualCamera.LookAt = parentTransform;
            if (virtualCamera.LookAt == null)
            {
                CoreLogging.LogError($"Parent transform not present. Fatal Error in {gameObject.name}.");
            }

            virtualCamera.m_XAxis.m_MaxSpeed = xCameraSpeed;
            virtualCamera.m_YAxis.m_MaxSpeed = yCameraSpeed;
        }

        public void CameraProcess()
        {
            
        }

        public void OnLook()
        {
            
        }
    }
}
