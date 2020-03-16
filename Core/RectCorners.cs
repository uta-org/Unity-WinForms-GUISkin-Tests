public struct RectCorners
{
    public int UpLeft { get; set; }
    public int UpRight { get; set; }
    public int BottomLeft { get; set; }
    public int BottomRight { get; set; }

    public RectCorners(int upLeft, int upRight, int bottomLeft, int bottomRight)
    {
        UpLeft = upLeft;
        UpRight = upRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
    }
}