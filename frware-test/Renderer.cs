using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class Renderer
    {
        // Window size of the renderer
        IntVector2 size;

        public static DoubleDrawingBuffer bufferFront = new DoubleDrawingBuffer(size);

        public Renderer(IntVector2 size)
        {
            this.size = size;
        }

        public void draw(Window window)
        {
            
        }
    }
}
