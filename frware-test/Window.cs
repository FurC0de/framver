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
        IntVector2 size;
        IntVector2 minSize = new IntVector2(8, 3);

        // Position of the top-left corner of this window
        IntVector2 position;

        // Title of the window
        String title = "Untitled";

        // FIXME: NOT USED YET
        int zindex = 1;

        // FIXME: NOT USED YET
        // Is this window expanded. This means window will flood all the free space.
        bool expanded = false;

        public DrawingChar[][] data;

        public Window(IntVector2 size, IntVector2 position) {}
        public Window() {}

        public void initialize() {
            setSize(new IntVector2(0, 0));
            setPosition(new IntVector2(0, 0));
        }

        public void setSize(IntVector2 size) {
            this.size = size;
            data = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.X, size.Y });
        }

        public void setPosition(IntVector2 position) {
            this.position = position;
        }

        public void setTitle(String title) {
            this.title = title;
        }

        public void getLine(int index) { 
        }
    }
}
