using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteRendererExtension
{
    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        var color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
