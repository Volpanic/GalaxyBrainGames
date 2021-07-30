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
    [SerializeField] private GameObject landingPointIdecator;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private Collider myCollider;
    [SerializeField] private LayerMask touchableLayer;


    [SerializeField] private Gradient successfulJumpGradiant;
    [SerializeField] private Gradient unsuccessfulJumpGradiant;

    private Camera cam;

    // Start is called before the first frame update
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
        if (controller.SelectedAndNotMoving)
        {
            //Jumping mode
            if (Input.GetKey(KeyCode.LeftControl) && curveMovement < 0)
            {
                if (!selectingJump)
                {
                    selectingJump = true;
                    startCurve = transform.position;
                    endCurve = startCurve + transform.forward;
                    endCursor = endCurve + transform.forward;
                    validJump = true;
                    curveMovement = -1;

                    MoveJumpCursor(Vector3.zero);
                    lRenderer.enabled = true;
                    if(landingPointIdecator != null) landingPointIdecator.SetActive(true);
                }

                ControlJumpCursor();

            }
            else if (curveMovement < 0)
            {
                selectingJump = false;

                lRenderer.enabled = false;
                if(landingPointIdecator != null) landingPointIdecator.SetActive(false);

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
            controller.transform.position = BezierCurve(curveMovement);

            CheckTouchable();

            if (curveMovement > 1)
            {
                curveMovement = -1;
                controller.CheckAndAttachToAnchorPoint(Vector3.zero, true);
                controller.ManualMove = true;
            }
        }
    }

    private void CheckTouchable()
    {
        Collider[] colliders = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents,transform.rotation,touchableLayer);

        if (colliders != null && colliders.Length != 0)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                Touchable touch = colliders[i].gameObject.GetComponent<Touchable>();
                if (touch != null) touch.OnTouch.Invoke();
            }
        }
    }

    private void ControlJumpCursor()
    {
        if (Input.GetKeyDown(KeyCode.D)) MoveJumpCursor(transform.forward);
        if (Input.GetKeyDown(KeyCode.A)) MoveJumpCursor(-transform.forward);
        if (Input.GetKeyDown(KeyCode.W)) MoveJumpCursor(-transform.right);
        if (Input.GetKeyDown(KeyCode.S)) MoveJumpCursor(transform.right);

        ClickInput();

        if (Input.GetKeyDown(KeyCode.Space) && validJump)
        {
            curveMovement = 0;
            controller.ManualMove = false;
            controller.DetachAnchorPoint();
            if (landingPointIdecator != null) landingPointIdecator.SetActive(false);
            selectingJump = false;

        }
    }

    private void ClickInput()
    {
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition,Camera.MonoOrStereoscopicEye.Mono);
        RaycastHit hit;

        if(Physics.Raycast(mouseRay,out hit,float.MaxValue,groundMask) && hit.normal.x < 0.5f && hit.normal.z < 0.5f)
        {
            //Vector3 targetPoint = controller.SnapToTileXZ(hit.point);
            Vector3 targetPoint = hit.point;
            //targetPoint.y = controller.CorrectYPos(targetPoint.y);

            validJump = false;
            lRenderer.colorGradient = unsuccessfulJumpGradiant;
            AdjustCursor(targetPoint);
            UpdateLineRenderer();
        }
    }

    private void MoveJumpCursor(Vector3 offset)
    {
        endCursor += offset;
        RaycastHit hit;
        endCursor.y = transform.position.y + (maxJumpDistance);
        bool didHit = Physics.Raycast(endCursor, Vector3.down, out hit, float.MaxValue, groundMask);
        validJump = false;
        lRenderer.colorGradient = unsuccessfulJumpGradiant;

        if (hit.collider != null)
        {
            AdjustCursor(hit.point);
        }
        else
        {
            endCurve.x = endCursor.x;
            endCurve.z = endCursor.z;

            if (landingPointIdecator != null)
            {
                landingPointIdecator.transform.position = endCurve;
            }
        }

        UpdateLineRenderer();
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

        //Set landing poing indecator to right place
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
