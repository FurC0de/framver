using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class DrawingLayer
    {
        public DoubleDrawingBuffer Buffer;

        public bool Visible = true;

        public bool Updated = false;

        public DrawingLayer(IntVector2 size)
        {
            Buffer = new DoubleDrawingBuffer(size);
        }


    }
}
