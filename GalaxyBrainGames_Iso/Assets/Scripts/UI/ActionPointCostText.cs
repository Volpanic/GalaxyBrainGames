using GalaxyBrain.Audio;
using GalaxyBrain.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Volpanic.Easing;
using Volpanic.UITweening;

namespace GalaxyBrain.UI
{
    public class ActionPointCostText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private GridPathfinding pathfinding;
        [SerializeField] private RectTransform canvasRootRect;
        [SerializeField] private AudioData changedSound;

        [SerializeField] private Gradient cheapToExpensiveGradient;

        private Camera cam;
        private RectTransform myRectTransform;

        private void Awake()
        {
            cam = Camera.main;
            myRectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (pathfinding != null)
                pathfinding.OnPathChanged += PathChanged;
        }

        private void OnDisable()
        {
            if (pathfinding != null)
                pathfinding.OnPathChanged -= PathChanged;
        }

        private void PathChanged()
        {
            if(costText != null)
            {
                int pathCount = pathfinding.GetPathCount()-1;

                if (pathCount > 0)
                {
                    //Get the point that the end of path matches on the canvas
                    Vector2 canvasPos = WorldPositonToCanvasPosition(pathfinding.GetPathEndPoint());

                    //Transition colour based on path cost
                    float gradiantSamplePos = Mathf.Clamp01(pathCount / 10f);

                    //Set text and colour
                    costText.text = "-" + pathCount.ToString();
                    costText.color = cheapToExpensiveGradient.Evaluate(gradiantSamplePos);

                    //Move transform
                    myRectTransform.position = canvasPos;
                    changedSound?.Play();
                }
                else
                {
                    costText.text = " ";
                }
            }
        }

        //Converts world point to canvas space, in different function in case we need to change later
        private Vector2 WorldPositonToCanvasPosition(Vector3 worldPos)
        {
            return cam.WorldToScreenPoint(worldPos);
        }
    }
}
