using System;
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

    private RectCorners? Corners { get; set; }
    private int? BorderSize { get; set; }
    private Color? BorderColor { get; set; }

    private TextureWorker()
    {
    }

    public TextureWorker(int width, int height)
    {
        Texture = new Texture2D(width, height);
    }

    public TextureWorker Fill(Color color)
    {
        int h = Texture.height;

        for (int x = 0; x < Texture.width; x++)
            for (int y = 0; y < Texture.height; y++)
            {
                if (BorderColor.HasValue && BorderSize.HasValue)
                {
                    if (x < BorderSize)
                    {
                        Texture.SetPixel(x, y, BorderColor.Value);
                        continue;
                    }

                    if (Texture.width - x <= BorderSize)
                    {
                        Texture.SetPixel(x, y, BorderColor.Value);
                        continue;
                    }

                    if (y < BorderSize)
                    {
                        Texture.SetPixel(x, h - y - 1, BorderColor.Value);
                        continue;
                    }

                    if (h - y - 1 < BorderSize)
                    {
                        Texture.SetPixel(x, h - y - 1, BorderColor.Value);
                        continue;
                    }
                }

                Texture.SetPixel(x, y, color);
            }

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

    public TextureWorker SmartFill(RectInt rect, Color color)
        => SmartFill(rect.xMin, rect.xMax, rect.yMin, rect.yMax, color);

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

    // TODO: DrawAnnulusSector
    // TODO: DrawAnnulus

    // Test methods
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

        Corners = new RectCorners(borderRadius, borderRadius, borderRadius, borderRadius);

        return this;
    }

    public TextureWorker FillRoundedBorders(Color color, RectCorners corners, Color? background = null)
    {
        int w = Texture.width;
        int h = Texture.height;

        int r0 = corners.UpLeft;
        int r1 = corners.UpRight;
        int r2 = corners.BottomLeft;
        int r3 = corners.BottomRight;

        //new RectOffset(left, right, top, bottom)

        if (r0 > 0)
            DrawnPixels += TextureUtils.DrawSector(Texture, r0, r0, r0, color, GetAngle(Corner.UpLeft), true, false);

        if (r1 > 0)
            DrawnPixels += TextureUtils.DrawSector(Texture, w - r1, r1, r1, color, GetAngle(Corner.UpRight), true, false);

        if (r2 > 0)
            DrawnPixels += TextureUtils.DrawSector(Texture, r2, h - r2, r2, color, GetAngle(Corner.BottomLeft), true, false);

        if (r3 > 0)
            DrawnPixels += TextureUtils.DrawSector(Texture, w - r3, h - r3, r3, color, GetAngle(Corner.BottomRight), true, false);

        SmartFill(r0, w - r1, r2, h - r3, color);

        SmartFill(r0, w - r1, 0, Mathf.Max(r0, r1), color);
        SmartFill(0, Mathf.Max(r0, r2), r0, h - r2, color);
        SmartFill(w - Mathf.Max(r1, r3), w, r1 - 1, h - r3 + 1, color);
        SmartFill(r2, w - r3 + 1, h - Mathf.Max(r2, r3), h, color);

        Corners = corners;

        return this;
    }

    // TODO: Create overload with xMin, xMax, yMin, yMax... Move code from above

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, int borderRadius, Color? background = null)
    //{
    //}

    //public TextureWorker FillRoundedBorders(Rect rect, Color color, RectOffset borderOffsets, Color? background = null)
    //{
    //}

    /// <summary>
    /// Set the borders (this must be called before Fill method).
    /// </summary>
    /// <param name="borderColor"></param>
    /// <param name="borderSize"></param>
    /// <returns></returns>
    public TextureWorker SetBorders(Color borderColor, int borderSize)
    {
        if (!Corners.HasValue)
        {
            BorderColor = borderColor;
            BorderSize = borderSize;

            return this;
        }

        // TODO: Do then rounded borders
        return this;
    }

    //public TextureWorker SetBorders(Color borderColor, RectOffset borderOffsets)
    //{
    //}

    //public TextureWorker SetBorders(Color rightBorderColor, Color upBorderColor, Color rightBorderColor, Color bottomBorderColor, RectOffset borderOffsets)
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