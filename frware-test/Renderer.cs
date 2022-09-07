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

        public DoubleDrawingBuffer buffer;

        public Renderer(IntVector2 size)
        {
            this.size = size;
            buffer = new DoubleDrawingBuffer(size);
        }

        public void addLine(IntVector2 coords, (string color, string letter)[] values)
        {
            DrawingChar[] line = values.Select(x => new DrawingChar(x.letter, x.color));
            buffer.drawLine(coords, line);
        }

        public void draw()
        {
            List<Tuple<IntVector2, int>> toUpdate = buffer.checkDirty();
            buffer.acceptDirty();

            for(Tuple<IntVector2, int> sector in toUpdate)
            {
                Console.SetCursorPosition(sector.Item1.X, sector.Item1.Y);
                Console.WriteLine(buffer.drawingChars[sector.Item1.X][sector.Item1.Y]);
            }
        }
    }
}
