using UnityEngine;

public class ScreenUtils
{
    public static Vector2 ScreenSizeInUnits(Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var height = camera.orthographicSize*2;
        var width = camera.aspect * height;

        return new Vector2(width, height);
    }
}
