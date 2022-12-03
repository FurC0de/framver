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
        public DoubleDrawingBuffer Buffer;

        public List<LayeringContainer> LayeringContainers = new List<LayeringContainer>();

        public Renderer(IntVector2 size)
        {
            Buffer = new DoubleDrawingBuffer(size);
        }
        public void DrawLine(IntVector2 coords, DrawingChar[] values)
        {
            Buffer.DrawLine(coords, values);
        }

        public void DrawLine(IntVector2 coords, (string color, char letter)[] values)
        {
            DrawingChar[] line = values.Select(x => new DrawingChar(x.letter, x.color)).ToArray();
            Buffer.DrawLine(coords, line);
        }

        public void DrawLine(IntVector2 coords, (string color, string word) value)
        {
            DrawingChar[] line = value.word.Select(x => new DrawingChar(x, value.color)).ToArray();
            Buffer.DrawLine(coords, line);
        }

        public void DrawWindow(Window window)
        {
            Buffer.DrawLine(window.Position, "Unsupported".ToDrawingCharArray());
        }

        public void AddLayeringContainer(LayeringContainer layeringContainer)
        {
            LayeringContainers.Add(layeringContainer);
        }

        public void Draw()
        {
            bool anyUpdates = false;
            
            foreach (LayeringContainer layeringContainer in LayeringContainers)
            {
                if (layeringContainer.Updated)
                    anyUpdates = true;

                if (anyUpdates)
                    Buffer += layeringContainer.Buffer;
            }


            List<Tuple<IntVector2, int>> toUpdate = Buffer.CheckDirty();
            //foreach (Tuple<IntVector2, int> tpl in toUpdate)
                //System.Diagnostics.Debug.WriteLine("update dirty: x{0} y{1}, len {2}",tpl.Item1.X, tpl.Item1.Y, tpl.Item2);

            Buffer.AcceptDirty();

            foreach(Tuple<IntVector2, int> sector in toUpdate)
            {
                Console.SetCursorPosition(sector.Item1.X, sector.Item1.Y);
                Console.Write(Buffer.GetLine(sector.Item1, sector.Item2));
            }
        }
    }
}
