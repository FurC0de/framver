using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class Window
    {
        // Size is only used if window is not expanded. aka windowed mode.
        IntVector2 Size;
        readonly IntVector2 MinSize = new IntVector2(8, 3);

        // Position of the top-left corner of this window.
        IntVector2 Position;

        // Title of the window
        String Title = "Untitled";

        // FIXME: NOT USED YET
        int ZIndex = 1;

        // FIXME: NOT USED YET
        // Is this window expanded. This means window will flood all the free space.
        bool Expanded = false;

        // Window contents
        public DrawingChar[][]? Data;

        // Window border style
        public WindowBorderStyle BorderStyle = new WindowBorderStyle("V", "H", '1', '2', '3', '4');

        public Window(IntVector2 size, IntVector2 position) {}
        public Window() {}

        public void Init() {
            SetSize(MinSize);
            SetPosition(new IntVector2(0, 0));
        }

        public void SetSize(IntVector2 size) {
            this.Size = size;
            Data = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.X, size.Y });
        }

        public void SetPosition(IntVector2 position) {
            this.Position = position;
        }

        public void SetTitle(String title) {
            this.Title = title;
        }

        public void GetLine(int index) { 
        }
    }
}
