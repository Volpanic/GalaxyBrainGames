using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GalaxyBrain
{
    public class CameraRotate : MonoBehaviour
    {
        public bool toggleInvert = false;

        public float mouseSensitivity = 5f;
        public float mouseScrollSpeed = 2f;
        public float keyScrollSpeed = 0.2f;

        public CinemachineVirtualCamera vcam;

        // Update is called once per frame
        void Update()
        {
            var orbital = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;


            if (toggleInvert == false)
            {
                orbital.m_XAxis.Value -= Input.mouseScrollDelta.y * mouseScrollSpeed;
            }
            if (toggleInvert == true)
            {
                orbital.m_XAxis.Value += Input.mouseScrollDelta.y * mouseScrollSpeed;
            }
            

            if (Input.GetMouseButton(1))
            {
                if(toggleInvert == false)
                {
                    orbital.m_XAxis.m_InputAxisValue = -mouseX;
                }
                else
                {
                    orbital.m_XAxis.m_InputAxisValue = mouseX;
                }

            }

            if (Input.GetMouseButtonUp(1))
            {
                orbital.m_XAxis.m_InputAxisValue = 0;
            }

            if (toggleInvert == false)
            {
                if (Input.GetKey("a"))
                {
                    orbital.m_XAxis.Value += keyScrollSpeed;
                }

                if (Input.GetKey("d"))
                {
                    orbital.m_XAxis.Value -= keyScrollSpeed;
                }
            }
            if (toggleInvert == true)
            {
                if (Input.GetKey("a"))
                {
                    orbital.m_XAxis.Value -= keyScrollSpeed;
                }

                if (Input.GetKey("d"))
                {
                    orbital.m_XAxis.Value += keyScrollSpeed;
                }
            }


        }
    }
}
