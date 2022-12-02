using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class Window
    {
        // Size is only used if window is not expanded. aka windowed mode.
        public IntVector2 Size;
        internal readonly static IntVector2 s_MinSize = new IntVector2(8, 3);

        // Position of the top-left corner of this window.
        public IntVector2 Position;

        // Title of the window
        String Title = "Untitled";

        // FIXME: NOT USED YET
        int ZIndex = 1;

        // FIXME: NOT USED YET
        // Is this window expanded. This means window will flood all the free space.
        bool Expanded = false;

        // Window contents.
        public DrawingChar[][]? Data;

        // Window border.
        public WindowBorder Border;

        public Window(IntVector2 size, IntVector2 position) {
            Size = size;
            Position = position;
            Border = new WindowBorder(s_MinSize);
        }

        public void Init() {
            SetSize(Size);
            SetPosition(Position);
            Border.Update();
        }

        public void SetSize(IntVector2 size) {
            this.Size = size;
            Data = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.X, size.Y });
            Border.SetSize(size);
        }

        public void SetPosition(IntVector2 position) {
            this.Position = position;
        }

        public void SetTitle(String title) {
            this.Title = title;
            Border.SetTitle(title);
            Border.Update();
        }
    }
}
