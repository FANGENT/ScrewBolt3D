using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
    public static ModelController Instance;
    public Transform Model;
    public Camera mainCamera; // Assign your main camera in inspector

    [Header("Zoom Settings")]
    public Slider zoomSlider;
    public float minZoom = 20f;  // Closest zoom
    public float maxZoom = 60f;  // Farthest zoom


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
        if (mainCamera == null) mainCamera = Camera.main;

        // Optional: hook the slider if not set manually
        if (zoomSlider != null)
        {
            zoomSlider.minValue = 0f;
            zoomSlider.maxValue = 1f;
            zoomSlider.value = 0f; // start at middle zoom
        }

        InitializeTheGame();
    }

    public void InitializeTheGame()
    {
        //modelAttributes = Model.GetComponent<ModelAttributes>();
        modelAttributes = LevelManager.Instance.currentModel.GetComponent<ModelAttributes>();
        BoltContainerManager.Instance.InitializeAllContainers();
    }

    private void Update()
    {
        if (mainCamera == null || zoomSlider == null) return;

        // Normalize slider value (0–1)
        float normalizedValue = zoomSlider.value;

        // Map slider directly to zoom range
        float targetFOV = Mathf.Lerp(maxZoom, minZoom, normalizedValue);

        // Instantly set FOV (no smoothness)
        mainCamera.fieldOfView = targetFOV;


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

                //--A fix rotation issue when reload level
                if (Model == null)
                {
                    Model = LevelManager.Instance.currentModel.transform;
                }

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

            if (Model == null)
            {
                Model = LevelManager.Instance.currentModel.transform;
            }

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

        if (Placement == null)
        {
            Debug.LogError("GameIsOver");
            return;
        }

        Bolt.GetComponent<Collider>().enabled = false;
        Bolt.transform.parent = null; // free it first
        CheckIfAnyPartOfModelCanFall();

        Sequence seq = DOTween.Sequence();

        // 1? POP UP (0.2s)
        seq.Append(Bolt.transform.DOMove(Bolt.transform.position + Vector3.up * 0.3f, 0.2f)
            .SetEase(Ease.OutBack));

        // 2? ROTATE while hovering (0.3s)
        seq.Join(Bolt.transform.GetChild(0).DOLocalRotate(new Vector3(1080, 0, 0), 0.3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear));

        // 3? MOVE toward placement (0.5s)
        Vector3 midPoint = (Bolt.transform.position + Placement.position) / 2f + Vector3.up * 0.2f;
        seq.Append(Bolt.transform.DOPath(
            new Vector3[] { Bolt.transform.position, midPoint, Placement.position },
            0.5f,
            PathType.CatmullRom
        ).SetEase(Ease.InOutSine));

        // Rotate while flying
        seq.Join(Bolt.transform.DORotate(new Vector3(0, 270, 0), 0.5f, RotateMode.Fast));

        // On Complete
        seq.OnComplete(() =>
        {
            Bolt.transform.SetParent(Placement);

            if (Bolt.GetComponentInParent<ExtraContainer>())
            {
                Bolt.transform.localScale = Vector3.one * 300f;
            }
            else
            {
                Bolt.transform.localScale = Vector3.one * 2;
            }

            Bolt.gameObject.layer = LayerMask.NameToLayer("UI");
            Bolt.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
        });

        if (SoundManager.Instance)
        {
            SoundManager.Instance.PlaySFX("screw");
        }
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
