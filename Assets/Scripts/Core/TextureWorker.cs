using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public sealed class TextureWorker
{
    public enum Corner
    {
        UpLeft,
        UpRight,
        BottomLeft,
        BottomRight
    }

    public Texture2D Texture { get; }
    public int DrawnPixels { get; private set; }

    private TextureWorker()
    {
    }

    public TextureWorker(int width, int height)
    {
        Texture = new Texture2D(width, height);
    }

    public TextureWorker Fill(Color color)
    {
        for (int x = 0; x < Texture.width; x++)
            for (int y = 0; y < Texture.height; y++)
                Texture.SetPixel(x, y, color);

        return this;
    }

    public TextureWorker Fill(Rect rect, Color color)
    {
        for (int x = 0; x < Texture.width; x++)
            for (int y = 0; y < Texture.height; y++)
            {
                var pos = new Vector2(x, y);
                if (rect.Contains(pos))
                    Texture.SetPixel(x, y, color);
            }

        return this;
    }

    public TextureWorker SmartFill(Rect rect, Color color)
        => SmartFill((int)rect.xMin, (int)rect.xMax, (int)rect.yMin, (int)rect.yMax, color);

    public TextureWorker SmartFill(int xMin, int xMax, int yMin, int yMax, Color color)
    {
        if (xMin >= xMax)
            throw new ArgumentException("xMin >= xMax");

        if (yMin >= yMax)
            throw new ArgumentException("yMin >= yMax");

        for (int x = xMin; x < xMax; x++)
            for (int y = yMin; y < yMax; y++)
            {
                Texture.SetPixel(x, Texture.height - y - 1, color);
                ++DrawnPixels;
            }

        return this;
    }

    // TODO: Create SmartFill
    // TODO: DrawAnnulusSector
    // TODO: DrawAnnulus

    public TextureWorker DrawSector(int radius, Range angles, Color color)
    {
        DrawnPixels += TextureUtils.DrawSector(Texture, radius, color, angles);
        return this;
    }

    public TextureWorker DrawCircle(int radius, Color color)
    {
        DrawnPixels += TextureUtils.Polar(Texture, radius, radius, radius);
        return this;
    }

    public TextureWorker CreateRoundedBorders(Color color, int borderRadius, Color? background = null)
    {
        int w = Texture.width;
        int h = Texture.height;
        int r = borderRadius;

        DrawnPixels += TextureUtils.DrawSector(Texture, r, r, r, color, GetAngle(Corner.UpLeft), true, false);
        DrawnPixels += TextureUtils.DrawSector(Texture, w - r, r, r, color, GetAngle(Corner.UpRight), true, false);
        DrawnPixels += TextureUtils.DrawSector(Texture, r, h - r, r, color, GetAngle(Corner.BottomLeft), true, false);
        DrawnPixels += TextureUtils.DrawSector(Texture, w - r, h - r, r, color, GetAngle(Corner.BottomRight), true, false);

        SmartFill(r, w - r, r, h - r, color);

        SmartFill(r, w - r, 0, r, color);
        SmartFill(0, r, r, h - r, color);
        SmartFill(w - r, w, r - 1, h - r + 1, color);
        SmartFill(r, w - r + 1, h - r, h, color);

        return this;
    }

    //public TextureWorker FillRoundedBorders(Color color, RectOffset borderOffsets, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, int borderRadius, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, RectOffset borderOffsets, Color? background = null)
    //{
    //}

    // TODO: Rename to ApplyBorders
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
        Texture.Apply();
        return this;
    }

    [Obsolete]
    public Texture2D GetTexture(bool apply = false)
    {
        if (apply)
            Texture.Apply();

        return Texture;
    }

    public static Range GetAngle(Corner corner)
    {
        switch (corner)
        {
            case Corner.UpLeft:
                return new Range(180, 270);

            case Corner.UpRight:
                return new Range(270, 360);

            case Corner.BottomLeft:
                return new Range(90, 180);

            case Corner.BottomRight:
                return new Range(0, 90);
        }

        return default;
    }
}