using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    [Header("Information related to the indicator")]
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Color _baseColor;
    [SerializeField] private float _baseSize;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnGUI()
    {
        if (!gameObject.activeInHierarchy) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        bool IsRenderable = (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height) && screenPos.x >= 0 && screenPos.x <= Screen.width;

        if (IsRenderable)
        {
            Vector3 indicatorPos = screenPos;

            var farAwayScale = 10;
            var closeScale = 30;

            var yPos = screenPos.y;
            var farOffPos = Screen.height * 2f;

            // Calculate the scale based on the distance from the camera
            var scale = Mathf.Lerp(farAwayScale, closeScale, Mathf.InverseLerp(farOffPos, 0, yPos));
            float scaledIndicatorSize = _baseSize * scale;
            float halfSize = scaledIndicatorSize / 2;

            // Clamp the indicator position within the screen boundaries
            indicatorPos.x = Mathf.Clamp(indicatorPos.x, halfSize, Screen.width - halfSize);

            // Calculate the y position based on the scaled indicator size
            var minYPos = halfSize;
            var maxYPos = Screen.height - halfSize;
            indicatorPos.y = Mathf.Clamp(indicatorPos.y, minYPos, maxYPos);

            Vector2 guiPos = new Vector2(indicatorPos.x, Screen.height - indicatorPos.y);

            Rect indicatorRect = new Rect(guiPos.x - halfSize, guiPos.y - halfSize, scaledIndicatorSize, scaledIndicatorSize);
            GUI.color = _baseColor;
            GUI.DrawTexture(indicatorRect, _texture);
        }
    }
}