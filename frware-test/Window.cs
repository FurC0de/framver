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
        IntVector2 size = new IntVector2(2,2);

        // Position of the top-left corner of this window
        IntVector2 position = new IntVector2(0, 0);

        // NOT USED YET
        int zindex = 1;

        // Is this window expanded. This means window will flood all the free space.
        bool expanded = false;


        public Window(IntVector2 size, IntVector2 position) { 
            this.size = size;
            this.position = position;
        }
    }
}
