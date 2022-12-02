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

        public List<DrawingLayer> Layers = new List<DrawingLayer>();

        public Renderer(IntVector2 size)
        {
            Buffer = new DoubleDrawingBuffer(size);
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
            if (window.Data == null)
                return;

            Buffer.DrawChar(window.Position, (DrawingChar)window.Border.CornerTopLeft);
            Buffer.DrawChar(window.Position + (window.Size.X-1, 0), (DrawingChar)window.Border.CornerTopRight);
            Buffer.DrawChar(window.Position + (window.Size.X-1, window.Size.Y-1), (DrawingChar)window.Border.CornerBottomRight);
            Buffer.DrawChar(window.Position + (0, window.Size.Y-1), (DrawingChar)window.Border.CornerBottomLeft);

            Buffer.DrawVLine(window.Position + (0, 1), window.Border.Left);
            Buffer.DrawVLine(window.Position + (window.Size.X-1, 1), window.Border.Right);

            Buffer.DrawLine(window.Position + (1, 0), window.Border.Top);
            Buffer.DrawLine(window.Position + (1, window.Size.Y-1), window.Border.Bottom);


            //foreach (DrawingChar[] line in window.Data) {
            //    buffer.DrawLine(coords + (1,0), line);
            //}

            // buffer.drawLine(coords, line);
        }

        public void AddLayer(DrawingLayer layer)
        {
            Layers.Add(layer);
        }

        public void Draw()
        {
            bool anyUpdates = false;
            
            foreach (DrawingLayer layer in Layers)
            {
                if (layer.Updated)
                    anyUpdates = true;

                if (anyUpdates)
                    Buffer += layer.Buffer;
            }


            List<Tuple<IntVector2, int>> toUpdate = Buffer.CheckDirty();
            //foreach (Tuple<IntVector2, int> tpl in toUpdate)
                //System.Diagnostics.Debug.WriteLine("update dirty: x{0} y{1}, len {2}",tpl.Item1.X, tpl.Item1.Y, tpl.Item2);

            Buffer.AcceptDirty();

            foreach(Tuple<IntVector2, int> sector in toUpdate)
            {
                //Console.SetCursorPosition(0, 0);
                //Console.WriteLine(sector.Item2);
                Console.SetCursorPosition(sector.Item1.X, sector.Item1.Y);
                Console.Write(Buffer.GetLine(sector.Item1, sector.Item2));
            }
        }
    }
}
