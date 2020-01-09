using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public sealed class TextureWorker
{
    internal enum Corner
    {
        UpLeft,
        UpRight,
        BottomLeft,
        BottomRight
    }

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

    // TODO: Create SmartFill
    // TODO: DrawAnnulusSector
    // TODO: DrawAnnulus

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

    // TODO: Rename from Fill to Create
    public TextureWorker CreateRoundedBorders(Color color, int borderRadius, Color? background = null)
    {
        int w = MyTexture.width;
        int h = MyTexture.height;
        int r = borderRadius;

        //TextureUtils.DrawSector(MyTexture, r, r, r, color, GetAngle(Corner.UpLeft), true, false);
        //TextureUtils.DrawSector(MyTexture, w - r, r, r, color, GetAngle(Corner.UpRight), true, false);
        //TextureUtils.DrawSector(MyTexture, r, h - r, r, color, GetAngle(Corner.BottomLeft), true, false);
        //TextureUtils.DrawSector(MyTexture, w - r, h - r, r, color, GetAngle(Corner.BottomRight), true, false);

        // TODO: Fix this
        TextureUtils.DrawSector(MyTexture, r, r, r, Color.red, GetAngle(Corner.UpLeft), true, false);
        TextureUtils.DrawSector(MyTexture, w - r, r, r, Color.green, GetAngle(Corner.UpRight), true, false);
        TextureUtils.DrawSector(MyTexture, r, h - r, r, Color.blue, GetAngle(Corner.BottomLeft), true, false);
        TextureUtils.DrawSector(MyTexture, w - r, h - r, r, Color.magenta, GetAngle(Corner.BottomRight), true, false);

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
        MyTexture.Apply();
        return this;
    }

    public Texture2D GetTexture(bool apply = false)
    {
        if (apply)
            MyTexture.Apply();

        return MyTexture;
    }

    private static Range GetAngle(Corner corner)
    {
        switch (corner)
        {
            case Corner.UpLeft:
                return new Range(180, 270);

            case Corner.UpRight:
                return new Range(270, 0);

            case Corner.BottomLeft:
                return new Range(90, 180);

            case Corner.BottomRight:
                return new Range(0, 90);
        }

        return default;
    }
}