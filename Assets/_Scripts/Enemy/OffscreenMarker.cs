using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public Texture2D indicatorTexture;
    public float indicatorSize = 50f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnGUI()
    {
        if (!gameObject.activeInHierarchy) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        if (screenPos.z > 0 && (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height))
        {
            Vector3 indicatorPos = screenPos;

            // Clamp the indicator position within the screen boundaries
            indicatorPos.x = Mathf.Clamp(indicatorPos.x, indicatorSize / 2, Screen.width - indicatorSize / 2);
            indicatorPos.y = Mathf.Clamp(indicatorPos.y, indicatorSize / 2, Screen.height - indicatorSize / 2);

            // Convert screen position to GUI space
            Vector2 guiPos = new Vector2(indicatorPos.x, Screen.height - indicatorPos.y);

            Rect indicatorRect = new Rect(guiPos.x - indicatorSize / 2, guiPos.y - indicatorSize / 2, indicatorSize, indicatorSize);
            GUI.DrawTexture(indicatorRect, indicatorTexture);
        }
    }
}