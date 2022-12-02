namespace frware_test
{
    internal class WindowBorder
    {
        public IntVector2 Size;

        public DrawingChar[]? Left;

        public DrawingChar[]? Top;

        public DrawingChar[]? Right;

        public DrawingChar[]? Bottom;

        public DrawingChar? CornerTopLeft;

        public DrawingChar? CornerTopRight;

        public DrawingChar? CornerBottomRight;

        public DrawingChar? CornerBottomLeft;

        public String? Title;

        public WindowBorderStyle Style = new WindowBorderStyle("V", "H", '1', '2', '3', '4');

        public WindowBorder(IntVector2 size)
        {
            this.Size = size;
        }

        public void SetSize(IntVector2 size)
        {
            this.Size = size;
        }

        public void SetStyle(WindowBorderStyle style)
        {
            this.Style = style;
        }

        public void SetTitle(String title)
        {
            this.Title = title;
        }

        public void Update()
        {
            if (Size.X < 3)
                throw new Exception("Window dimensions are too small to draw a border (Size.X < 3)");

            if (Size.Y < 3)
                throw new Exception("Window dimensions are too small to draw a border (Size.Y < 2)");

            Left = new DrawingChar[Size.Y - 2];
            Right = new DrawingChar[Size.Y - 2];

            // TODO: Add pattern alternating.
            for (int y = 0; y < Size.Y - 2; y++)
            {
                Left[y] = new DrawingChar(Style.Vertical[0], "#FFFFFF");
                Right[y] = new DrawingChar(Style.Vertical[0], "#FFFFFF");
            }

            Top = new DrawingChar[Size.X - 2];
            Bottom = new DrawingChar[Size.X - 2];

            // TODO: Add pattern alternating.
            for (int x = 0; x < Size.X - 2; x++)
            {
                Top[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
                Bottom[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
            }

            CornerTopLeft = new DrawingChar(Style.CornerTopLeft, "#FFFFFF");
            CornerTopRight = new DrawingChar(Style.CornerTopRight, "#FFFFFF");
            CornerBottomRight = new DrawingChar(Style.CornerBottomRight, "#FFFFFF");
            CornerBottomLeft = new DrawingChar(Style.CornerBottomLeft, "#FFFFFF");
        }
    }
}