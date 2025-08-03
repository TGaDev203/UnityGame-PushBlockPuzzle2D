using System;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public Vector2 InputDirection { get; private set; }

    [SerializeField] private RectTransform joystickOuter;
    [SerializeField] private RectTransform joystickInner;
    private Vector2 startPos;
    // private Vector2 inputDirection;
    private float activeTouchId = -1;

    private void Update()
    {
        TouchHandler();
    }

    private void TouchHandler()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (TouchOnJoystickArea(touch.position))
                    {
                        activeTouchId = touch.fingerId;
                        startPos = touch.position;
                        joystickOuter.position = startPos;
                        joystickInner.position = startPos;
                        joystickOuter.gameObject.SetActive(true);
                    }
                    startPos = touchPos;

                    break;

                case TouchPhase.Moved:
                    Vector2 delta = touchPos - startPos;
                    float radius = joystickOuter.sizeDelta.x / 2f;
                    Vector2 rawInput = Vector2.ClampMagnitude(delta, radius) / radius;

                    if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y)) InputDirection = new Vector2(Mathf.Sign(rawInput.x), 0);
                    else InputDirection = new Vector2(0, Mathf.Sign(rawInput.y));

                    joystickInner.position = startPos + InputDirection * radius * 2f;
                    break;

                // case TouchPhase.Stationary:
                //     if (touch.fingerId == activeTouchId)
                //     {
                //         // Vector2 touchPos = touch.position;
                //         Vector2 delta = touchPos - startPos;
                //         float radius = joystickOuter.sizeDelta.x / 2f;

                //         inputDirection = Vector2.ClampMagnitude(delta, radius) / radius;

                //         float joystickMovementRange = radius * 2f;
                //         joystickInner.position = startPos + inputDirection * joystickMovementRange;
                //     }
                //     break;

                case TouchPhase.Ended:
                    if (touch.fingerId == activeTouchId)
                    {
                        InputDirection = Vector2.zero;
                        joystickOuter.gameObject.SetActive(false);
                    }
                    break;

                // case TouchPhase.Canceled:
                //     if (touch.fingerId == activeTouchId)
                //     {
                //         activeTouchId = -1;
                //         joystickInner.position = startPos;
                //     }
                    // break;
            }
        }
    }

    private bool TouchOnJoystickArea(Vector2 pos)
    {
        return pos.x < Screen.width / 2f;
    }

    // public Vector2 GetInputDirection()
    // {
    //     return inputDirection;
    // }
}