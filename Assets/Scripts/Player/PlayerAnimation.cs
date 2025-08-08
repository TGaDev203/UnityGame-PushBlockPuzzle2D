using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void SetIsWalking(bool value)
    {
        playerAnimator.SetBool("isWalking", value);
    }

    public void SetDirection(Vector2Int input)
    {
        int direction = 0;
        if (input == Vector2Int.up) direction = 1;
        else if (input == Vector2Int.right) direction = 2;
        else if (input == Vector2Int.left) direction = 3;

        playerAnimator.SetFloat("Direction", direction);
    }
}