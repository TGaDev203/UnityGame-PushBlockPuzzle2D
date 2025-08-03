using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveTime = 0.2f;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private JoystickController joystickController;
    private Vector2Int currentGridPos;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
        currentGridPos = Vector2Int.RoundToInt(transform.position / cellSize);
    }

    private void Update()
    {
        if (!isMoving && joystickController.InputDirection != Vector2.zero)
        {
            Vector2Int dir = Vector2Int.RoundToInt(joystickController.InputDirection);
            Vector2Int newGridPos = currentGridPos + dir;

            targetPosition = new Vector3(newGridPos.x, newGridPos.y, 0) * cellSize;
            StartCoroutine(MoveToPostion(targetPosition));
            currentGridPos = newGridPos;
        }
    }

    private IEnumerator MoveToPostion(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            transform.position = Vector3.Lerp(start, target, elapsed / moveTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}
