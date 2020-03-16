using UnityEngine;

public struct Polar
{
    public float r, theta;

    public float deg => (theta * Mathf.Rad2Deg + 360) % 360;

    public Polar(float r, float theta)
    {
        this.r = r;
        this.theta = theta;
    }

    public static implicit operator Vector2(Polar p)
    {
        return new Vector2(p.r * Mathf.Cos(p.theta), p.r * Mathf.Sin(p.theta));
    }

    public static implicit operator Polar(Vector2 p)
    {
        return new Polar(p.magnitude, Mathf.Atan2(p.y, p.x));
    }

    public override string ToString()
    {
        return $"Deg: {deg:F2} | Radius: {r:F2}";
    }
}