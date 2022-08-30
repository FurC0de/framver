using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class DoubleDrawingBuffer
    {
        IntVector2 size = new IntVector2(2, 2);

        DrawingChar[][] drawingChars;
        DrawingChar[][] oldDrawingChars;
        DrawingCharState[][] states;

        public DoubleDrawingBuffer(IntVector2 size) {
            this.size = size;
            this.drawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(size.X, size.Y);
            this.oldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(size.X, size.Y);
            this.states = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(size.X, size.Y);
        }

        public void pushBuffer()
        {

        }
    }
}
