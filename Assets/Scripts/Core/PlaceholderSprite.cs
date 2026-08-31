using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序化占位美术：运行时生成纯色方块 Sprite。
/// 这样 M1 阶段不需要任何外部图片资源，就能看到角色、敌人、子弹。
/// 后续换正式素材时，直接替换 SpriteRenderer 的 sprite 即可。
/// </summary>
public static class PlaceholderSprite
{
    // 按 (尺寸, 颜色) 缓存，避免每个子弹都新建一张纹理（会漏内存）
    private static readonly Dictionary<(int size, Color32 color), Sprite> cache =
        new Dictionary<(int size, Color32 color), Sprite>();

    /// <summary>生成一个纯色方块 Sprite，ppu = size，即 1 世界单位 = size 像素。</summary>
    public static Sprite Create(Color color, int size = 32)
    {
        (int, Color32) key = (size, color);
        if (cache.TryGetValue(key, out Sprite cached))
            return cached;

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        cache[key] = sprite;
        return sprite;
    }
}
