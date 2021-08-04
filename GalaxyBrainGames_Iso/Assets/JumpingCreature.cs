using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FreeMoveController))]
public class JumpingCreature : MonoBehaviour
{
    [SerializeField] private FreeMoveController controller;
    [SerializeField] private float maxJumpDistance = 4;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LineRenderer lRenderer;
    [SerializeField] private GameObject landingPointIdecator;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private CharacterController myCollider;
    [SerializeField] private LayerMask touchableLayer;


    [SerializeField] private Gradient successfulJumpGradiant;
    [SerializeField] private Gradient unsuccessfulJumpGradiant;

    private Camera cam;

    //Start is called before the first frame update
    void Awake()
    {
        if (controller == null)
        {
            Debug.LogWarning("Controller not attached to jumping creature! on " + gameObject.name);
            enabled = false;
        }

        cam = Camera.main;

        if (landingPointIdecator != null) landingPointIdecator.SetActive(true);

        creatureData.LogCreature(controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.Selected)
        {
            //Jumping mode
            if ((Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)) || curveMovement < 0)
            {
                if (!selectingJump)
                {
                    selectingJump = true;
                    endCurve = startCurve + transform.forward;
                    endCursor = endCurve + transform.forward;
                    validJump = true;
                    curveMovement = -1;

                    lRenderer.enabled = true;
                    if (landingPointIdecator != null) landingPointIdecator.SetActive(true);
                }

                startCurve = transform.position;
                ClickInput();
            }
            else if (curveMovement < 0)
            {
                selectingJump = false;

                lRenderer.enabled = false;
                if (landingPointIdecator != null) landingPointIdecator.SetActive(false);

            }
            else
            {
                selectingJump = false;
            }
        }
        else
        {
            selectingJump = false;
            lRenderer.enabled = false;
            if (landingPointIdecator != null) landingPointIdecator.SetActive(false);
        }


        if (curveMovement >= 0)
        {
            lRenderer.enabled = true;
            curveMovement += Time.deltaTime;
            controller.AttemptMove(BezierCurve(curveMovement));

            CheckForBottom();

            if (curveMovement >= 1)
            {
                curveMovement = -1;
            }

            CheckTouchable();
        }
        else
        {
            curveMovement = -1;
        }
    }

    private void CheckForBottom()
    {
        Collider[] cols = Physics.OverlapBox(myCollider.bounds.center,myCollider.bounds.extents*1.1f,transform.rotation,groundMask);

        foreach(Collider col in cols)
        {
            if(controller.AttemptAttachToCarry(col.gameObject))
            {
                curveMovement = -1;
                break;
            }
        }
    }

    private void CheckTouchable()
    {
        Collider[] colliders = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, transform.rotation, touchableLayer);

        if (colliders != null && colliders.Length != 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Touchable touch = colliders[i].gameObject.GetComponent<Touchable>();
                if (touch != null) touch.OnTouch.Invoke();
            }
        }
    }

    private void ClickInput()
    {
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        RaycastHit hit;

        if (Physics.Raycast(mouseRay, out hit, float.MaxValue, groundMask) && hit.normal.x < 0.5f && hit.normal.z < 0.5f)
        {
            //Vector3 targetPoint = controller.SnapToTileXZ(hit.point);
            Vector3 targetPoint = hit.point;
            //targetPoint.y = controller.CorrectYPos(targetPoint.y);

            validJump = false;
            lRenderer.colorGradient = unsuccessfulJumpGradiant;
            AdjustCursor(targetPoint);
            UpdateLineRenderer();

            if(validJump && Input.GetMouseButtonUp(2) && (myCollider.isGrounded || controller.IsBeingCarried()))
            {
                curveMovement = 0;
                lRenderer.enabled = false;
                controller.DetachCarry();
            }
        }
    }

    private void AdjustCursor(Vector3 hitPoint)
    {
        endCurve = hitPoint;
        endCurve.y = controller.CorrectYPos(endCurve.y);

        float dist = Vector3.Distance(transform.position, hitPoint);
        if (dist <= maxJumpDistance)
        {
            validJump = true;

            handle = (startCurve + endCurve) * 0.5f;
            handle.y += Mathf.Min(1, maxJumpDistance - dist);
            lRenderer.colorGradient = successfulJumpGradiant;
        }

        //Set landing point indecator to right place
        if (landingPointIdecator != null)
        {
            landingPointIdecator.transform.position = hitPoint;
        }

        handle = (startCurve + endCurve) * 0.5f;
        handle.y += 8;
    }

    private Vector3[] lineRendererPoints = new Vector3[16];

    private void UpdateLineRenderer()
    {
        if (lRenderer != null)
        {
            lRenderer.positionCount = lineRendererPoints.Length;

            float incr = 1.0f / lineRendererPoints.Length;
            float cur = 0;
            for (var i = 0; i < lineRendererPoints.Length; i++)
            {
                lineRendererPoints[i] = BezierCurve(cur);
                cur += incr;
            }
            lineRendererPoints[lineRendererPoints.Length - 1] = BezierCurve(1);
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

    public void OnDrawGizmosSelected()
    {

        Gizmos.DrawSphere(startCurve, 0.25f);
        Gizmos.DrawSphere(endCurve, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(handle, 0.25f);

        Gizmos.color = Color.blue;
    }
}
