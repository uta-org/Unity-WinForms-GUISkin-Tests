using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public sealed class TextureWorker
{
    private Texture2D MyTexture { get; }

    private TextureWorker()
    {
    }

    public TextureWorker(int width, int height)
    {
        MyTexture = new Texture2D(width, height);
    }

    public TextureWorker Fill(Color color)
    {
        for (int x = 0; x < MyTexture.width; x++)
        {
            for (int y = 0; y < MyTexture.height; y++)
            {
                MyTexture.SetPixel(x, y, color);
            }
        }

        return this;
    }

    public TextureWorker Fill(Rect rect, Color color)
    {
        for (int x = 0; x < MyTexture.width; x++)
        {
            for (int y = 0; y < MyTexture.height; y++)
            {
                var pos = new Vector2(x, y);
                if (rect.Contains(pos))
                {
                    MyTexture.SetPixel(x, y, color);
                }
            }
        }

        return this;
    }

    public TextureWorker DrawSector(int radius, Range angles, Color color)
    {
        TextureUtils.DrawSector(MyTexture, radius, color, angles);
        return this;
    }

    public TextureWorker DrawCircle(int radius, Color color)
    {
        TextureUtils.Polar(MyTexture, radius, radius, radius);
        return this;
    }

    //public TextureWorker FillRoundedBorders(Color color, int borderRadius, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Color color, RectOffset borderOffsets, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, int borderRadius, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, RectOffset borderOffsets, Color? background = null)
    //{
    //}

    //public TextureWorker WithBorders(Color borderColor, int borderSize)
    //{
    //}

    //public TextureWorker WithBorders(Color borderColor, RectOffset borderOffsets)
    //{
    //}

    //public TextureWorker WithBorders(Color rightBorderColor, Color upBorderColor, Color rightBorderColor, Color bottomBorderColor, RectOffset borderOffsets)
    //{
    //}

    public TextureWorker Apply()
    {
        MyTexture.Apply();
        return this;
    }

    public Texture2D GetTexture(bool apply = false)
    {
        if (apply)
            MyTexture.Apply();

        return MyTexture;
    }
}