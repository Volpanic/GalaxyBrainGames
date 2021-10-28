using GalaxyBrain.Creatures;
using GalaxyBrain.Systems;
using UnityEngine;
using Volpanic.Easing;

namespace GalaxyBrain.Interactables
{
    /// <summary>
    /// Controls a block that can be pushed by the strong creature
    /// an amount of tiles infront of it.
    /// </summary>
    public class PushBlock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer pushBlockRenderer;
        [SerializeField] private CharacterController controller;
        [SerializeField] private Collider myCollider;
        [SerializeField] private CreatureData creatureData;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private int maxPushRange = 3;

        [SerializeField] private Color ViablePathColor = Color.green;
        [SerializeField] private Color UnvialbePathColor = Color.red;

        private Plane plane;
        private Camera cam;

        private bool moving = false;
        private bool pathLocked = false;
        private Vector3 startPos = Vector3.zero;
        private Vector3 targetPos = Vector3.zero;
        private Vector3 oldMovement = Vector3.zero;
        private Vector3 tempEndPoint = Vector3.zero;
        private float pushTimer = 0;
        private float pushMaxTime = 1;
        private bool viablePushPath = true;
        private bool firstSnap = true;
        private bool firstLand = true;

        public bool Moving
        {
            get { return moving; }
        }

        public bool PathLocked
        {
            get { return pathLocked; }
        }

        public void UpdatePlane()
        {
            plane.SetNormalAndPosition(Vector3.up, transform.position + (Vector3.down*0.5f));
        }

        // Start is called before the first frame update
        void Awake()
        {
            UpdatePlane();
            cam = Camera.main;
        }

        private void FixedUpdate()
        {
            if (moving)
            {
                UpdateBlockMoving();
            }
            else
            {
                if (Physics.BoxCast(controller.bounds.center, controller.bounds.extents * 0.98f, Vector3.down, Quaternion.identity, 0.025f)) 
                {
                    //SmoothSnapToGrid();

                    if (!firstLand)
                    {
                        firstLand = true;
                        creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min - Vector3.one, myCollider.bounds.max + Vector3.one);
                    }
                }
                else
                {
                    firstLand = false;
                }
                controller.SimpleMove(Vector3.zero);
            }
        }

        private void UpdateBlockMoving()
        {
            pushTimer += Time.fixedDeltaTime;
            float lerpPos = Easingf.OutSine(0f, 1f, pushTimer / pushMaxTime);
            Vector3 target = Vector3.Lerp(startPos, targetPos, lerpPos);

            Vector3 movement = target - oldMovement;
            if (movement != Vector3.zero) controller.Move(movement);

            oldMovement = target;

            //Check if we hit a wall
            if (pushTimer >= 0.2f && (PlaceMeeting(movement, 0.9f) || !PlaceMeeting(Vector3.down*0.25f, 0.9f)))
            {
                moving = false;

                //Put snap to tile code here...
                Vector3 targetSnap = creatureData.pathfinding.ToGridPos(transform.position);
                targetSnap.y = transform.position.y;

                controller.enabled = false;
                transform.position = targetSnap;
                controller.enabled = true;
            }

            if (pushTimer >= pushMaxTime)
            {
                //Disable the controller to allow for manual movement.
                creatureData.pathfinding.UpdateNodeCells(myCollider.bounds.min - Vector3.one, myCollider.bounds.max + Vector3.one);

                creatureData.pathfinding.UpdateNodeCells(startPos - Vector3.one, startPos + Vector3.one);
                moving = false;
                //SmoothSnapToGrid();
            }
        }

        public bool PlaceMeeting(Vector3 offset, float sizeScale)
        {
            Collider[] colls = Physics.OverlapBox(controller.bounds.center + offset, controller.bounds.extents * sizeScale, Quaternion.identity, groundMask);

            for (int i = 0; i < colls.Length; i++)
            {
                //Skip if it's collider is mine or my childs
                if(colls[i].gameObject == gameObject || colls[i].transform.IsChildOf(transform))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void SmoothSnapToGrid()
        {
            Vector3 targetSnap = creatureData.pathfinding.ToGridPos(transform.position);

            controller.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, targetSnap, Time.fixedDeltaTime * 4);
            controller.enabled = true;

            if (!firstSnap)
            {
                creatureData.pathfinding.UpdateNodeCells(controller.bounds.min - Vector3.one, controller.bounds.max + Vector3.one);
                firstSnap = true;
            }
        }

        public float UpdateAbility(PlayerController controller,Vector3 interactionCardinal)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float enter = 0;
            viablePushPath = true;

            if (plane.Raycast(ray, out enter))
            {
                Vector3 hit = ray.GetPoint(enter);

                Vector3 endPoint = new Vector3(Mathf.Ceil(hit.x), transform.position.y, Mathf.RoundToInt(hit.z)) - transform.position;
                endPoint.x *= interactionCardinal.normalized.x;
                endPoint.z *= interactionCardinal.normalized.z;
                endPoint = Vector3.ClampMagnitude(endPoint, maxPushRange).magnitude * interactionCardinal;

                //Point has Changed
                if (endPoint != tempEndPoint)
                {
                    viablePushPath = CheckIfPathIsViable(interactionCardinal.normalized, Mathf.FloorToInt(endPoint.magnitude));

                    //Update Grid
                    if (pushBlockRenderer != null)
                    {
                        UpdateTileIdecator(interactionCardinal.normalized, endPoint.magnitude);
                    }
                }

                if (viablePushPath && controller.LeftClicked)
                {
                    pushBlockRenderer.gameObject.SetActive(false);
                    tempEndPoint = Vector3.zero;

                    //Cancel out if too short
                    if (endPoint.magnitude <= 0.1f)
                    {
                        return endPoint.magnitude;
                    }

                    targetPos = (endPoint);
                    pathLocked = true;
                    return endPoint.magnitude;
                }

                //Cancel
                if(controller.RightClicked)
                {
                    pushBlockRenderer.gameObject.SetActive(false);
                    tempEndPoint = Vector3.zero;
                    return -1;
                }
            }
            return 0;
        }

        private bool CheckIfPathIsViable(Vector3 normalized, int magnitude)
        {
            Vector3 offset = normalized;

            //Loop through each point on the path
            for(int i = 0; i < magnitude; i++)
            {
                //Check for a wall
                if (PlaceMeeting(offset,0.95f))
                {
                    return false;
                }

                //Check if there is no ground below us
                if (CheckIfOffMap(transform.position + offset, 16))
                {
                    return false;
                }

                offset += normalized; //Increase by 1 tile size
            }


            return true;
        }

        private bool CheckIfOffMap(Vector3 position, float downDistance)
        {
            return !Physics.BoxCast(position, controller.bounds.extents * 0.95f, Vector3.down, transform.rotation, downDistance, groundMask);
        }

        private void UpdateTileIdecator(Vector3 normalized, float magnitude)
        {
            if(magnitude <= .9f)
            {
                pushBlockRenderer.transform.localPosition = new Vector3(0, pushBlockRenderer.transform.localPosition.y,0);
                return;
            }

            pushBlockRenderer.gameObject.SetActive(true);
            normalized.y = 0;
            
            Vector3 endDirection = normalized * magnitude;
            endDirection.x = Mathf.Abs(endDirection.x);
            endDirection.z = Mathf.Abs(endDirection.z);

            pushBlockRenderer.size = new Vector2(Mathf.Max(endDirection.x, 1), Mathf.Max(endDirection.z, 1));

            //Set correct position
            Vector3 midPoint = ((normalized * magnitude) * 0.5f) + (normalized * 0.5f);
            pushBlockRenderer.transform.localPosition = new Vector3(midPoint.x,pushBlockRenderer.transform.localPosition.y,midPoint.z);

            //Change Color
            pushBlockRenderer.color = (viablePushPath) ? ViablePathColor : UnvialbePathColor; 
        }

        public void StartPush()
        {
            startPos = Vector3.zero;
            pushTimer = 0;
            oldMovement = Vector3.zero;
            moving = true;
            firstLand = false;
            firstSnap = false;

            UpdateTileIdecator(Vector3.zero, 0);

            pathLocked = false;
            pushMaxTime = 1;
        }
    }
}