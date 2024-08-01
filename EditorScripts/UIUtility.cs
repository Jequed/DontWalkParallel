using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static bool InImage(Vector3 position, Image image, out Vector3 bottomLeft, out Vector3 topRight)
    {
        Vector3[] corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);

        bottomLeft = image.canvas.worldCamera.WorldToScreenPoint(corners[0]);
        topRight = image.canvas.worldCamera.WorldToScreenPoint(corners[2]);

        return position.x >= bottomLeft.x && position.x <= topRight.x && position.y >= bottomLeft.y && position.y <= topRight.y;
    }
}