using UnityEngine;
public enum InputStatus{
    Tap,
    Swipe,
    NoTouch
}
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    [Header("Swipe Detection Settings")]
    public float swipeThreshold = 50f;     // Minimum distance in pixels to be considered a swipe
    public float maxTapTime = 0.2f;        // Max time in seconds for a tap

    public InputStatus inputStatus = InputStatus.NoTouch;
    private Vector2 startTouchPosition;
    private float startTime;
    private bool isTouching = false;
    private bool swipeDetected = false;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
//#if UNITY_EDITOR
//        // Mouse input for editor
//        if (Input.GetMouseButtonDown(0))
//        {
//            startTouchPosition = Input.mousePosition;
//            startTime = Time.time;
//            isTouching = true;
//            swipeDetected = false;
//        }
//        else if (Input.GetMouseButton(0) && isTouching && !swipeDetected)
//        {
//            float duration = Time.time - startTime;
//            Vector2 currentPosition = Input.mousePosition;
//            float distance = Vector2.Distance(startTouchPosition, currentPosition);

//            if (duration >= maxTapTime)
//            {
//                DetectTouchType(startTouchPosition, currentPosition, duration);
//                swipeDetected = true;
//            }
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            if (!swipeDetected && isTouching)
//            {
//                Vector2 endTouchPosition = Input.mousePosition;
//                float duration = Time.time - startTime;
//                DetectTouchType(startTouchPosition, endTouchPosition, duration);
//            }
//            isTouching = false;
//        }
//        else if(!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0))
//        {
//            inputStatus = InputStatus.NoTouch;
//        }
//#else
//        // Touch input for mobile
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            switch (touch.phase)
//            {
//                case TouchPhase.Began:
//                    startTouchPosition = touch.position;
//                    startTime = Time.time;
//                    isTouching = true;
//                    swipeDetected = false;
//                    break;

//                case TouchPhase.Moved:
//                case TouchPhase.Stationary:
//                    if (isTouching && !swipeDetected)
//                    {
//                        float duration = Time.time - startTime;
//                        float distance = Vector2.Distance(startTouchPosition, touch.position);

//                        if (duration >= maxTapTime)
//                        {
//                            DetectTouchType(startTouchPosition, touch.position, duration);
//                            swipeDetected = true;
//                        }
//                    }
//                    break;

//                case TouchPhase.Ended:
//                    if (!swipeDetected && isTouching)
//                    {
//                        float duration = Time.time - startTime;
//                        DetectTouchType(startTouchPosition, touch.position, duration);
//                    }
//                    isTouching = false;
//                    break;
//            }
//        }
//        else
//        {
//            inputStatus = InputStatus.NoTouch;
//        }
//#endif
    }

    void DetectTouchType(Vector2 start, Vector2 end, float duration)
    {
        float distance = Vector2.Distance(start, end);

        if (distance < swipeThreshold)
        {
            if (duration < maxTapTime)
            {
                //Debug.Log("Tap detected");
                inputStatus = InputStatus.Tap;
                GameObject bolt = ModelController.Instance.DetectUnderlyingObject(Input.mousePosition);
                if (bolt)
                {
                    ModelController.Instance.UnScrewBolt(bolt);
                }
            }
            else
            {
                //Debug.Log("Swipe detected (due to long hold)");
                inputStatus = InputStatus.Swipe;
            }
        }
        else
        {
            Vector2 direction = (end - start).normalized;
            //Debug.Log($"Swipe detected. Direction: {direction}");
            inputStatus = InputStatus.Swipe;
        }
    }
}