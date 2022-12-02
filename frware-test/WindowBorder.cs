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

        private int TitleStart = 1;

        public WindowBorderStyle Style = new WindowBorderStyle("║", "═", '╔', '╗', '╝', '╚');
        //public WindowBorderStyle Style = new WindowBorderStyle("V", "H", '1', '2', '3', '4');

        public WindowBorder(IntVector2 size)
        {
            this.Size = size;
        }

        public void SetSize(IntVector2 size)
        {
            this.Size = size;
            Update();
        }

        public void SetStyle(WindowBorderStyle style)
        {
            this.Style = style;
            Update();
        }

        public void SetTitle(String title)
        {
            this.Title = title;
            Update();
        }

        public void Update()
        {
            if (Size.X < 3)
                throw new Exception("Window dimensions are too small to draw a border (Size.X < 3)");

            if (Size.Y < 3)
                throw new Exception("Window dimensions are too small to draw a border (Size.Y < 2)");

            // Create left and right borders.

            Left = new DrawingChar[Size.Y - 2];
            Right = new DrawingChar[Size.Y - 2];

            // TODO: Add pattern alternating.
            for (int y = 0; y < Size.Y - 2; y++)
            {
                Left[y] = new DrawingChar(Style.Vertical[0], "#FFFFFF");
                Right[y] = new DrawingChar(Style.Vertical[0], "#FFFFFF");
            }

            // Create top and bottom borders.

            Top = new DrawingChar[Size.X - 2];
            Bottom = new DrawingChar[Size.X - 2];

            // TODO: Add pattern alternating.
            for (int x = 0; x < Size.X - 2; x++)
            {
                // Adding window title to the top border (if title not null).
                if (Title != null) {
                    // Check if we have enough space for indentation

                    // +T+
                    // +Ti+
                    // +Tit+
                    // +Titl+
                    // +Title+
                    // +Title-+
                    // +-Title-+
                    // +-Title--+
                    // +-Title---+

                    if (Size.X - Title.Length > 3)
                    {
                        if (x >= TitleStart && x < TitleStart+Title.Length)
                        {
                            Top[x] = new DrawingChar(Title[x-TitleStart], "#FFFFFF");
                        } else
                        {
                            Top[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
                        }
                    } else 
                    {
                        if (x < Title.Length)
                        {
                            Top[x] = new DrawingChar(Title[x], "#FFFFFF");
                        } else
                        {
                            Top[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
                        }
                    }
                } else
                {
                    Top[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
                }

                Bottom[x] = new DrawingChar(Style.Horizontal[0], "#FFFFFF");
            }

            // Create corners.

            CornerTopLeft = new DrawingChar(Style.CornerTopLeft, "#FFFFFF");
            CornerTopRight = new DrawingChar(Style.CornerTopRight, "#FFFFFF");
            CornerBottomRight = new DrawingChar(Style.CornerBottomRight, "#FFFFFF");
            CornerBottomLeft = new DrawingChar(Style.CornerBottomLeft, "#FFFFFF");
        }
    }
}