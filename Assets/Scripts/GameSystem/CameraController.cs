using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
namespace SaveTheCat
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform virtualCamTransform;
        private CinemachineVirtualCamera mainVirCam;
        private Transform playerTransform;
        private CinemachineVirtualCamera[] virtualCameras;
        private CinemachineBrain cinemachineBrain;
        private bool isInTransistion = false;
        public bool IsInTransistion => isInTransistion;
        public void Initialize()
        {
            DontDestroyOnLoad(virtualCamTransform.gameObject);
            mainVirCam = GetComponentInChildren<CinemachineVirtualCamera>();

            cinemachineBrain = GetComponent<CinemachineBrain>();
            playerTransform = GameObject.FindWithTag("Player").transform;
            virtualCameras = virtualCamTransform.GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (var vircam in virtualCameras)
            {
                vircam.Follow = playerTransform;
                vircam.LookAt = playerTransform;
            }
        }
        public void ResetCameraVir()
        {
            StopAllCoroutines();
            cinemachineBrain.enabled = false;
            cinemachineBrain.enabled = true;
        }
        public void Camera_MainMenu()
        {
            ResetCameraVir();
            isInTransistion=true;
            Initialize();
            CamPriorityReset();
            mainVirCam.enabled=true;
            mainVirCam.Priority = 10;
            isInTransistion=false;
        }
        public void CamPriorityReset()
        {
            int camCount = virtualCameras.Length;
            for (int i = 0; i < camCount; i++)
            {
                virtualCameras[i].Priority = camCount - i;
            }
        }
        public void CamPriorityPlayer()
        {
            CamPriorityReset();
            virtualCameras.First(p => p.gameObject.name == "VirCamMain").Priority = 10;
        }
        public void CamPriorityPlayerUp()
        {
            CamPriorityReset();
            virtualCameras.First(p => p.gameObject.name == "VirCamMainUp").Priority = 10;
        }
        public void Camera_GameStart()
        {
            ResetCameraVir();
            mainVirCam.enabled = false;
            isInTransistion = true;
            CamPriorityReset();
            StartCoroutine(Coroutine_TsunamiShowcase());
        }


        private IEnumerator Coroutine_TsunamiShowcase()
        {
            
            int index = 0;
            int topPriority = virtualCameras.Length + 1;
            yield return new WaitForSeconds(0.2f);
            virtualCameras[index++].Priority = topPriority + index;
            yield return new WaitForSeconds(0.5f); //Cut
            virtualCameras[index++].Priority = topPriority + index;
            yield return new WaitForSeconds(0.5f);
            virtualCameras[index++].Priority = topPriority + index; //Front fallback
            yield return new WaitForSeconds(1.5f);
            virtualCameras[index++].Priority = topPriority + index; //To Main
            yield return new WaitForSeconds(2f);
            CamPriorityPlayer();
            yield return new WaitForSeconds(0.5f);
            isInTransistion = false;
        }
        public void FinishShowcase()
        {
            // CamPriorityReset();
            virtualCameras[virtualCameras.Length - 1].Priority = 10;
        }
    }
}

