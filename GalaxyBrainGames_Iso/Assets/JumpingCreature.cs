using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class JumpingCreature : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private float maxJumpDistance = 4;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LineRenderer lRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        if(controller == null)
        {
            Debug.LogWarning("Controller not attached to jumping creature! on " + gameObject.name);
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.SelectedAndNotMoving)
        {
            //Jumping mode
            if(Input.GetKey(KeyCode.LeftControl) && curveMovement < 0)
            {
                if(!selectingJump)
                {
                    selectingJump = true;
                    startCurve = transform.position;
                    endCurve = startCurve;
                    endCursor = endCurve;
                    validJump = true;
                    curveMovement = -1;
                }

                ControlJumpCursor();

            }
            else
            {
                selectingJump = false;
            }
        }
        else
        {
            selectingJump = false;
        }


        if (curveMovement >= 0)
        {
            curveMovement += Time.deltaTime;
            controller.transform.position = BezierCurve(curveMovement);

            if (curveMovement > 1)
            {
                curveMovement = -1;
                controller.CheckAndAttachToAnchorPoint(Vector3.zero,true);
                controller.ManualMove = true;
            }
        }
    }

    private void ControlJumpCursor()
    {
        if (Input.GetKeyDown(KeyCode.D)) MoveJumpCursor(transform.forward);
        if (Input.GetKeyDown(KeyCode.A)) MoveJumpCursor(-transform.forward);
        if (Input.GetKeyDown(KeyCode.W)) MoveJumpCursor(-transform.right);
        if (Input.GetKeyDown(KeyCode.S)) MoveJumpCursor(transform.right);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            curveMovement = 0;
            controller.ManualMove = false;
            controller.DetachAnchorPoint();
        }
    }

    private void MoveJumpCursor(Vector3 offset)
    {
        endCursor += offset;
        RaycastHit hit;
        endCursor.y = transform.position.y + (maxJumpDistance / 2);
        bool didHit = Physics.Raycast(endCursor,Vector3.down, out hit,float.MaxValue,groundMask);
        validJump = false;

        if (hit.collider != null)
        {
            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist <= maxJumpDistance)
            {
                endCurve = hit.point;
                endCurve.y = controller.CorrectYPos(endCurve.y);
                validJump = true;

                handle = (startCurve + endCurve) * 0.5f;
                handle.y += Mathf.Min(1,maxJumpDistance-dist);
            }

            handle = (startCurve + endCurve) * 0.5f;
            handle.y += 8;

            UpdateLineRenderer();
        }
    }

    private Vector3[] lineRendererPoints = new Vector3[16];

    private void UpdateLineRenderer()
    {
        if(lRenderer != null)
        {
            lRenderer.positionCount = lineRendererPoints.Length;

            float incr = 1.0f / lineRendererPoints.Length;
            float cur = 0;
            for (var i = 0; i < lineRendererPoints.Length; i++)
            {
                lineRendererPoints[i] = BezierCurve(cur);
                cur += incr;
            }
            lineRendererPoints[lineRendererPoints.Length-1] = BezierCurve(1);
            lRenderer.SetPositions(lineRendererPoints);
        }
    }

    private Vector3 startCurve = Vector3.zero;
    private Vector3 endCurve = Vector3.zero;
    private Vector3 endCursor = Vector3.zero;
    private Vector3 handle = Vector3.zero;
    private bool selectingJump = false;

    private float curveMovement = -1;
    private bool isFollowingPath = false;
    private bool validJump = false;

    public void JumpTowards(Vector3 target)
    {
        Debug.Log("Jump Towards");
        startCurve = transform.position;
        endCurve = target;

        handle = (startCurve + endCurve) * 0.5f;
        handle.y += 8;

        curveMovement = 0;
        isFollowingPath = true;
    }

    Vector3 BezierCurve(float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * startCurve;
        p += 2 * u * t * handle;
        p += tt * endCurve;
        return p;
    }

    public void OnDrawGizmos()
    {

        Gizmos.DrawSphere(startCurve, 0.25f);
        Gizmos.DrawSphere(endCurve, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(handle, 0.25f);

        Gizmos.color = Color.blue;

        if (validJump)
        {
            //float incr = 1.0f / 8.0f;
            //float cur = 0;
            //for (var i = 0; i < 8; i++)
            //{
            //    Gizmos.DrawLine(BezierCurve(cur), BezierCurve(cur + incr));
            //    cur += incr;
            //}
        }
    }
}
