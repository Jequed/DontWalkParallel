using UnityEngine;

public static class InputUtility
{
    private static bool isMouse = false;
    private static bool isMouseDown = false;
    private static bool isMouseUp = false;
    private static bool isDoubleClick = false;

    private static bool isDoubleTouch = false;
    private static Vector2 initialTouchPosition1 = Vector3.zero;
    private static Vector2 initialTouchPosition2 = Vector3.zero;
    private static float pinchDelta = 0.0f;

    private static Vector3 mousePosition = Vector3.zero;

    private static float lastMouseDownTime = 0.0f;

    private const float doubleClickTime = 0.6f;

    public static Vector3 MousePosition
    {
        get
        {
            return mousePosition;
        }
    }

    public static float PinchDelta
    {
        get
        {
            return pinchDelta;
        }
    }

    public static void Update()
    {
        #if !UNITY_ANDROID
            UpdatePCInput();
        #else
            UpdateMobileInput();
        #endif

        if (isMouseDown)
        {
            if (Time.time - lastMouseDownTime < doubleClickTime)
                isDoubleClick = true;

            lastMouseDownTime = Time.time;
        }

        if (isMouseUp)
            isDoubleClick = false;
    }

    public static bool LeftMouse()
    {
        return isMouse;
    }
    public static bool LeftMouseDown()
    {
        return isMouseDown;
    }
    public static bool LeftMouseUp()
    {
        return isMouseUp;
    }

    public static bool RightMouse()
    {
        return Input.GetMouseButton(1);
    }

    public static bool MiddleMouse()
    {
        return Input.GetMouseButton(2);
    }

    public static bool DoubleClick()
    {
        return isDoubleClick;
    }

    public static bool DoubleTouch()
    {
        return isDoubleTouch;
    }

    private static void UpdatePCInput()
    {
        isMouse = Input.GetMouseButton(0);
        isMouseDown = Input.GetMouseButtonDown(0);
        isMouseUp = Input.GetMouseButtonUp(0);
        mousePosition = Input.mousePosition;
    }
    private static void UpdateMobileInput()
    {
        isMouse = false;
        isMouseDown = false;
        isMouseUp = false;

        if (Input.touches.Length == 1)
        {
            var touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                isMouseDown = true;
                isMouse = true;
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                isMouse = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isMouseUp = true;
            }

            mousePosition = touch.position;
        }
        else if (Input.touches.Length == 2)
        {
            if (!isDoubleTouch)
            {
                initialTouchPosition1 = Input.touches[0].position;
                initialTouchPosition2 = Input.touches[1].position;
            }

            isDoubleTouch = true;

            var distance1 = Vector2.Distance(initialTouchPosition1, initialTouchPosition2);
            var distance2 = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            if (distance1 > 0.0f)
                pinchDelta = distance2 / distance1;

            mousePosition = Input.touches[0].position;
        }

        if (Input.touches.Length != 2)
            isDoubleTouch = false;
    }
}