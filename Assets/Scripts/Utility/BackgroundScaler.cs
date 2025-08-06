using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();

        if (sp == null || sp.sprite == null) return;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        Vector2 spriteSize = sp.sprite.bounds.size;
        Vector3 scale = transform.localScale;

        scale.x = screenWidth / spriteSize.x;
        scale.y = screenHeight / spriteSize.y;

        transform.localScale = scale;
    }
}