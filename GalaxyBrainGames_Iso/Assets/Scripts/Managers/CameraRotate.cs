using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GalaxyBrain
{
    public class CameraRotate : MonoBehaviour
    { 
        
        public float mouseSensitivity = 10000f;

        public CinemachineVirtualCamera vcam;

        private void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var orbital = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            if (Input.GetMouseButton(1))
            {
                orbital.m_XAxis.m_InputAxisValue = mouseX;

            }
        
        }
    }
}