using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public static class TextureUtils
{
    public static int drawnPixels;

    private static void InitTexture(ref Texture2D texture, int width, int height, bool force = false)
    {
        if (texture != null && !force)
            return;

        texture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point
        };

        texture.SetPixels32(new Color32[width * height]);
    }

    public static Texture2D DrawPixel(this Texture2D texture, int x, int y, Func<int, int, Color?> color, bool apply = false)
    {
        InitTexture(ref texture, x, y);

        if (x < 0 || x > texture.width || y < 0 || y > texture.height)
            return texture;

        var c = color?.Invoke(x, y);

        if (c.HasValue)
        {
            texture.SetPixel(x, TransformToLeftTop_y(y, texture.height), c.Value);
            ++drawnPixels;
        }

        if (apply)
            texture.Apply();

        return texture;
    }

    /// <summary>
    /// Draws a pixel just like SetPixel except 0,0 is the left top corner.
    /// Takes the width and height as parameters - faster for calling this in a loop.
    /// </summary>
    /// <param name="width">Width of the target bitmap</param>
    /// <param name="height">Height of the target bitmap</param>
    public static Texture2D DrawPixel(this Texture2D texture, int x, int y, int width, int height, Color color, bool apply = false)
    {
        InitTexture(ref texture, width, height);

        if (x < 0 || x > width || y < 0 || y > height)
            return texture;

        texture.SetPixel(x, TransformToLeftTop_y(y, height), color);
        ++drawnPixels;

        if (apply)
            texture.Apply();

        return texture;
    }

    public static void DrawSector(Texture2D texture, int radius, Color color, Range angles, bool filled = true,
        bool apply = true)
        => Polar(texture, radius, radius, radius, (x, y) => SectorPredicate(x, y, radius, radius, color, angles), filled, apply);

    public static void DrawSector(Texture2D texture, int x, int y, int radius, Color color, Range angles, bool filled = true,
        bool apply = true)
        => Polar(texture, x, y, radius, (_x, _y) => SectorPredicate(_x, _y, x, y, color, angles), filled, apply);

    public static void DrawCircle(Texture2D texture, int radius, Color color, bool filled = true,
        bool apply = true)
        => Polar(texture, radius, radius, radius, (x, y) => color, filled, apply);

    public static void DrawCircle(Texture2D texture, int x, int y, int radius, Color color, bool filled = true,
        bool apply = true)
        => Polar(texture, x, y, radius, (_x, _y) => color, filled, apply);

    internal static void Polar(Texture2D texture, int x, int y, int radius, Func<int, int, Color?> predicate = null, bool filled = true, bool apply = true)
    {
        int cx = radius;
        int cy = 0;
        int radiusError = 1 - cx;

        if (predicate == null)
            predicate = EmptyPolarPredicate;

        while (cx >= cy)
        {
            if (!filled)
                PlotCircle(texture, cx, x, cy, y, predicate);
            else
                ScanLinePolar(texture, cx, x, cy, y, predicate);

            cy++;

            if (radiusError < 0)
                radiusError += 2 * cy + 1;
            else
            {
                cx--;
                radiusError += 2 * (cy - cx + 1);
            }
        }

        if (apply)
            texture.Apply();
    }

    private static Color? SectorPredicate(int x, int y, int ox, int oy, Color color, Range angles)
    {
        // TODO: This is rotated 90 degrees
        float minAngle = ClampAngle(angles.from);
        float maxAngle = ClampAngle(angles.count);

        Polar p = new Vector2(x - ox, y - oy);

        if (p.deg >= minAngle && p.deg < maxAngle)
            return color;

        return null;
    }

    // Only for testing purposes
    private static Color? EmptyPolarPredicate(int x, int y)
        => Color.red;

    private static void PlotCircle(Texture2D texture, int cx, int x, int cy, int y, Func<int, int, Color?> predicate)
    {
        texture.DrawPixel(cx + x, cy + y, predicate); // Point in octant 1...
        texture.DrawPixel(cy + x, cx + y, predicate);
        texture.DrawPixel(-cx + x, cy + y, predicate);
        texture.DrawPixel(-cy + x, cx + y, predicate);
        texture.DrawPixel(-cx + x, -cy + y, predicate);
        texture.DrawPixel(-cy + x, -cx + y, predicate);
        texture.DrawPixel(cx + x, -cy + y, predicate);
        texture.DrawPixel(cy + x, -cx + y, predicate); // ... point in octant 8
    }

    private static void ScanLinePolar(Texture2D texture, int cx, int x, int cy, int y, Func<int, int, Color?> predicate)
    {
        texture.DrawLine(cx + x, cy + y, -cx + x, cy + y, predicate);
        texture.DrawLine(cy + x, cx + y, -cy + x, cx + y, predicate);
        texture.DrawLine(-cx + x, -cy + y, cx + x, -cy + y, predicate);
        texture.DrawLine(-cy + x, -cx + y, cy + x, -cx + y, predicate);
    }

    public static void DrawLine(this Texture2D texture, Vector3 start, Vector3 end, Color color)
    {
        Line(texture, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color);
    }

    public static void DrawLine(this Texture2D texture, Vector2 start, Vector2 end, Color color)
    {
        Line(texture, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color);
    }

    public static void DrawLine(this Texture2D texture, int x0, int y0, int x1, int y1, Func<int, int, Color> predicate)
    {
        Line(texture, x0, y0, x1, y1, predicate);
    }

    public static void DrawLine(this Texture2D texture, int x0, int y0, int x1, int y1, Func<int, int, Color?> predicate)
    {
        Line(texture, x0, y0, x1, y1, predicate);
    }

    /// <summary>
    /// Draws a line between two points. Implementation of Bresenham's line algorithm.
    /// </summary>
    /// <param name="x0">x of the start point</param>
    /// <param name="y0">y of the start point</param>
    /// <param name="x1">x of the end point</param>
    /// <param name="y1">y of the end point</param>
    public static void DrawLine(this Texture2D texture, int x0, int y0, int x1, int y1, Color color, Func<int, int, bool> predicate = null)
    {
        Line(texture, x0, y0, x1, y1, color, predicate);
    }

    private static void Line(Texture2D texture, int x0, int y0, int x1, int y1, Func<int, int, Color> color)
    {
        if (color == null)
            throw new ArgumentException("color");

        int width = texture.width;
        int height = texture.height;

        bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (isSteep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int deltax = x1 - x0;
        int deltay = Math.Abs(y1 - y0);

        int error = deltax / 2;
        int ystep;
        int y = y0;

        if (y0 < y1)
            ystep = 1;
        else
            ystep = -1;

        for (int x = x0; x < x1; x++)
        {
            if (isSteep)
            {
                texture.DrawPixel(y, x, width, height, color(x, y));
            }
            else
            {
                texture.DrawPixel(x, y, width, height, color(x, y));
            }

            error = error - deltay;
            if (error < 0)
            {
                y = y + ystep;
                error = error + deltax;
            }
        }
    }

    private static void Line(Texture2D texture, int x0, int y0, int x1, int y1, Func<int, int, Color?> color)
    {
        int width = texture.width;
        int height = texture.height;

        bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (isSteep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int deltax = x1 - x0;
        int deltay = Math.Abs(y1 - y0);

        int error = deltax / 2;
        int ystep;
        int y = y0;

        if (y0 < y1)
            ystep = 1;
        else
            ystep = -1;

        for (int x = x0; x < x1; x++)
        {
            if (isSteep)
            {
                var c = color?.Invoke(x, y);

                if (c.HasValue)
                    texture.DrawPixel(y, x, width, height, c.Value);
            }
            else
            {
                var c = color?.Invoke(x, y);

                if (c.HasValue)
                    texture.DrawPixel(x, y, width, height, c.Value);
            }

            error = error - deltay;
            if (error < 0)
            {
                y = y + ystep;
                error = error + deltax;
            }
        }
    }

    private static void Line(Texture2D texture, int x0, int y0, int x1, int y1, Color color, Func<int, int, bool> predicate = null)
    {
        int width = texture.width;
        int height = texture.height;

        bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (isSteep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int deltax = x1 - x0;
        int deltay = Math.Abs(y1 - y0);

        int error = deltax / 2;
        int ystep;
        int y = y0;

        if (y0 < y1)
            ystep = 1;
        else
            ystep = -1;

        for (int x = x0; x < x1; x++)
        {
            if (isSteep)
            {
                if (predicate != null && predicate(x, y) || predicate == null)
                    texture.DrawPixel(y, x, width, height, color);
            }
            else
            {
                if (predicate != null && predicate(x, y) || predicate == null)
                    texture.DrawPixel(x, y, width, height, color);
            }

            error = error - deltay;
            if (error < 0)
            {
                y = y + ystep;
                error = error + deltax;
            }
        }
    }

    /// <summary>
    /// Swap two ints by reference.
    /// </summary>
    private static void Swap(ref int x, ref int y)
    {
        int temp = x;
        x = y;
        y = temp;
    }

    /// <summary>
    /// Transforms a point in the texture plane so that 0,0 points at left-top corner.</summary>
    private static int TransformToLeftTop_y(int y, int height)
    {
        return height - y;
    }

    /// <summary>
    /// Transforms a point in the texture plane so that 0,0 points at left-top corner.</summary>
    private static int TransformToLeftTop_y(float y, int height)
    {
        return height - (int)y;
    }

    private static float ClampAngle(float angle) => ClampAngle(angle, 0, 360);

    private static float ClampAngle(float angle, float from, float to)
    {
        if (angle > 180) angle = 360 - angle;
        angle = Mathf.Clamp(angle, from, to);
        if (angle < 0) angle = 360 + angle;

        return angle;
    }
}