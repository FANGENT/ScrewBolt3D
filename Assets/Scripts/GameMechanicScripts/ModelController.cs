using UnityEngine;
using DG.Tweening;

public class ModelController : MonoBehaviour
{
    public static ModelController Instance;
    public Transform Model;
    public ModelAttributes modelAttributes;
    //public Transform Target;
    public float rotationSpeed = 0.2f; // Adjust as needed
    private Vector3 lastMousePosition;
    private bool CanBeSwipe;
    public float swipeThreshold = 50f;     // Minimum distance in pixels to be considered a swipe
    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        InitializeTheGame();
    }

    public void InitializeTheGame()
    {
        modelAttributes = Model.GetComponent<ModelAttributes>();
        BoltContainerManager.Instance.InitializeAllContainers();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastMousePosition = touch.position;
            }
            // Only rotate while moving
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;

                // Horizontal swipe -> Y-axis rotation
                float yRotation = -delta.x * rotationSpeed;

                // Vertical swipe -> X-axis rotation
                float xRotation = delta.y * rotationSpeed;

                // Apply rotation to the object
                Model.Rotate(xRotation, yRotation, 0f, Space.World);
                //if (InputHandler.Instance.inputStatus == InputStatus.Swipe)
                //{
                    
                //}
            }
            if(touch.phase == TouchPhase.Ended)
            {
                if (Vector2.Distance(lastMousePosition, touch.position) < swipeThreshold)
                {
                    GameObject Bolt = DetectUnderlyingObject(touch.position);
                    if (Bolt)
                    {
                        ModelController.Instance.UnScrewBolt(Bolt);
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0)) // Left mouse button
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            

            Vector3 delta = Input.mousePosition - lastMousePosition;

            float yRotation = -delta.x * rotationSpeed * 0.5f;
            float xRotation = delta.y * rotationSpeed * 0.5f;

            Model.Rotate(xRotation, yRotation, 0f, Space.World);
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Vector2.Distance(lastMousePosition, Input.mousePosition) < swipeThreshold)
            {
                GameObject Bolt = DetectUnderlyingObject(Input.mousePosition);
                if (Bolt)
                {
                    ModelController.Instance.UnScrewBolt(Bolt);
                }
            }
        }
    }

    public GameObject DetectUnderlyingObject(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Screw");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log("Hit object: " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }
        return null;
    }
    GameObject lastBolt;
    public void UnScrewBolt(GameObject Bolt)
    {
        if (!Bolt.GetComponent<BlockedScrewsController>().CheckIfItCanBePulledOut())
        {
            Debug.LogError("BoltIsLocked");
            return;
        }
        Transform Placement = GetSpaceForNewlyUnscrewedBolt(Bolt.transform);

        if(Placement == null)
        {
            Debug.LogError("GameIsOver");
            return;
        }

        Bolt.GetComponent<Collider>().enabled = false;
        Bolt.transform.parent = Placement;
        CheckIfAnyPartOfModelCanFall();

        Sequence seq = DOTween.Sequence();
        seq.Join(Bolt.transform.GetChild(0).DOLocalRotate(new Vector3(1440, 0, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        seq.Join(Bolt.transform.DOMove(Bolt.transform.position - Bolt.transform.right * 0.5f, 0.2f).SetEase(Ease.Linear));


        seq.Append(Bolt.transform.DOMove(Placement.position + new Vector3(-0.2f, 0.3f, 0), 0.2f));
        seq.Join(Bolt.transform.DORotate(new Vector3(15, -65, -25), 0.2f, RotateMode.Fast).SetEase(Ease.Linear));

        seq.Append(Bolt.transform.DOMove(Placement.position, 0.2f));
        seq.Join(Bolt.transform.DORotate(new Vector3(0, 270, 0), 0.2f, RotateMode.Fast).SetEase(Ease.Linear));

        //seq.Play();
        //lastBolt = Bolt;

    }

    private void CheckIfAnyPartOfModelCanFall()
    {
        modelAttributes.MakeNecessaryPartsFall();
        modelAttributes.SetPivotForRotation();
    }

    private Transform GetSpaceForNewlyUnscrewedBolt(Transform UnscrewedBolt)
    {
        return BoltContainerManager.Instance.GetTargetWhereBoltShouldBePlaced(UnscrewedBolt);
    }
}
