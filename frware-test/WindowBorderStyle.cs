namespace frware_test
{
    public class WindowBorderStyle
    {
        public String Vertical;

        public String Horizontal;

        public Char CornerTopLeft;

        public Char CornerTopRight;

        public Char CornerBottomRight;

        public Char CornerBottomLeft;

        public WindowBorderStyle(String vertical, String horizontal, Char cornerTopLeft, Char cornerTopRight, Char cornerBottomRight, Char cornerBottomLeft)
        {
            Vertical = vertical;
            Horizontal = horizontal;
            CornerTopLeft = cornerTopLeft;
            CornerTopRight = cornerTopRight;
            CornerBottomRight = cornerBottomRight;
            CornerBottomLeft = cornerBottomLeft;
        }
    }
}