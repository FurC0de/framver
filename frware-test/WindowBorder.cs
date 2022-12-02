namespace frware_test
{
    internal class WindowBorder
    {
        public IntVector2 size;

        public DrawingChar[]? Left;

        public DrawingChar[]? Top;

        public DrawingChar[]? Right;

        public DrawingChar[]? Bottom;

        public WindowBorderStyle style = new WindowBorderStyle("V", "H", '1', '2', '3', '4');

        public WindowBorder(IntVector2 size)
        {
            this.size = size;
        }

        public void SetStyle(WindowBorderStyle style)
        {
            this.style = style;
        }
        public void SetSize(IntVector2 size)
        {
            this.size = size;
        }
    }
}