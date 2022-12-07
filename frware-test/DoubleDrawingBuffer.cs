using Pastel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace frware_test
{
    internal class DoubleDrawingBuffer
    {
        public IntVector2 Size { get; private set; }

        private volatile DrawingChar[][] DrawingChars;
        private volatile DrawingChar[][] OldDrawingChars;

        //private bool[][] _states;
        private volatile GenericSynchronizedAlternating<DenseArray<bool>> _states;

        public DoubleDrawingBuffer(IntVector2 size) {
            this.Size = size;
            this.DrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this.OldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this._states = new GenericSynchronizedAlternating<DenseArray<bool>>(
                new DenseArray<bool>(Size.Y, Size.X),
                new DenseArray<bool>(Size.Y, Size.X)
            );

            System.Diagnostics.Debug.WriteLine("created jagged arrays of length: dc({0},{1}) odc({2},{3}) s({4},{5})", 
                DrawingChars.Length, DrawingChars[0].Length,
                OldDrawingChars.Length, OldDrawingChars[0].Length,
                _states.UnsafeRead(1).Columns, _states.UnsafeRead(1).Rows);
        }

        public List<Tuple<IntVector2, int>> CheckDirty() {
            List<Tuple<IntVector2, int>> dirtySectors = new List<Tuple<IntVector2, int>>(); // Coords, length

            //System.Diagnostics.Debug.WriteLine($"-> x{_states[0].Length}*y{_states.Length}");
            //System.Diagnostics.Debug.WriteLine($"-> expecting x{Size.X}*y{Size.Y}");

            (DenseArray<bool> Value, uint Used) states = _states.LockAndRead();

            for (int y = 0; y < Size.Y; y++) {
                int db = -1;
                int de = -1;
                
                for (int x = 0; x < Size.X; x++) {
                    
                    if (states.Value[y,x]) {
                        if (db == -1)
                            db = x;
                    
                        if (de < x)
                            de = x;
                    }
                }

                if (db != -1) {
                    dirtySectors.Add(new Tuple<IntVector2, int>(new IntVector2(db, y), de - db + 1));
                    //System.Diagnostics.Debug.WriteLine("adding dirty sector: from {0} to {1} of len {2}, on coords ({3} {4})", db, de, de - db, db, y);
                }
            }

            _states.Unlock(states.Used);

            //System.Diagnostics.Debug.WriteLine($"<- x{_states[0].Length}*y{_states.Length}"); 
            //System.Diagnostics.Debug.WriteLine($"<- expecting x{Size.X}*y{Size.Y}");

            //System.Diagnostics.Debug.WriteLine($"Checked dirty sectors successfully");
            return dirtySectors;

        }

        public unsafe void AcceptDirty()
        {
            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();
            fixed (bool* a = &states.Value.GetArray()[0])
            {
                bool* b = a;
                var span = new Span<bool>(b, states.Value.Columns * states.Value.Rows);
                span.Fill(false);
            }
            //System.Diagnostics.Debug.Write($"a:dirt[{(states.Used == 1 ? "Front" : "Back")}],");
            _states.Unlock(states.Used);
        }

        //public unsafe void SetDirty(IntVector2 leftTop, IntVector2 rightBottom)
        //{
        //    (bool[][], uint) states = _states.LockAndModify();
        //    IntVector2 area = rightBottom - leftTop;

        //    fixed (bool* a = &states.Item1[leftTop.Y][leftTop.X])
        //    {
        //        bool* b = a;
        //        var span = new Span<bool>(b, area.X*area.Y);
        //        span.Fill(true);
        //    }
        //    _states.Unlock(states.Item2);
        //}

        public unsafe void SetGlobalDirty()
        {
            // PrintStatePointers();

            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();
            fixed (bool* a = &states.Value.GetArray()[0])
            {
                bool* b = a;
                var span = new Span<bool>(b, states.Value.Columns * states.Value.Rows);
                span.Fill(true);
            }
            _states.Unlock(states.Used);

            // PrintStatePointers();
        }

        public unsafe void PrintStatePointers()
        {
            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();

            for (int y = 0; y < Size.Y*Size.X; y+=Size.X)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    fixed (bool* a = &states.Value.GetArray()[y+x])
                    {
                        System.Diagnostics.Debug.Write($"{((uint)a):X8}-{(*a ? 1 : 0)} ");
                    }

                }
                System.Diagnostics.Debug.WriteLine("");
            }
            _states.Unlock(states.Used);
        }

        public void DrawChar(IntVector2 coords, DrawingChar character)
        {
            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();
            states.Value[coords.Y, coords.X] = true;
            _states.Unlock(states.Used);

            DrawingChars[coords.Y][coords.X] = character;
        }

        public void DrawLine(IntVector2 coords, DrawingChar[] line)
        {
            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();
            for (int x = 0; x < line.Length; x++) {
                states.Value[coords.Y, x + coords.X] = true;
            }
            _states.Unlock(states.Used);
            Array.Copy(line, 0, DrawingChars[coords.Y], coords.X, line.Length);
        }

        public void DrawVLine(IntVector2 coords, DrawingChar[] line)
        {
            (DenseArray<bool> Value, uint Used) states = _states.LockAndModify();
            for (int y = 0; y < line.Length; y++)
            {
                states.Value[y + coords.Y, coords.X] = true;
                DrawingChars[y + coords.Y][coords.X] = line[y];
            }
            _states.Unlock(states.Used);
        }

        public void DrawWindow(Window window)
        {
            if (window.Border != null)
            {
                if (window.Border.CornerTopLeft != null)
                    DrawChar(window.Position, (DrawingChar)window.Border.CornerTopLeft);
                if (window.Border.CornerTopRight != null)
                    DrawChar(window.Position + (window.Size.X - 1, 0), (DrawingChar)window.Border.CornerTopRight);
                if (window.Border.CornerBottomRight != null)
                    DrawChar(window.Position + (window.Size.X - 1, window.Size.Y - 1), (DrawingChar)window.Border.CornerBottomRight);
                if (window.Border.CornerBottomLeft != null)
                    DrawChar(window.Position + (0, window.Size.Y - 1), (DrawingChar)window.Border.CornerBottomLeft);

                if (window.Border.Left != null)
                    DrawVLine(window.Position + (0, 1), window.Border.Left);
                if (window.Border.Right != null)
                    DrawVLine(window.Position + (window.Size.X - 1, 1), window.Border.Right);
                if (window.Border.Top != null)
                    DrawLine(window.Position + (1, 0), window.Border.Top);
                if (window.Border.Bottom != null)
                    DrawLine(window.Position + (1, window.Size.Y - 1), window.Border.Bottom);
            }

            if (window.Data == null)
            {
                return;
            }
            //foreach (DrawingChar[] line in window.Data) {
            //    buffer.DrawLine(coords + (1,0), line);
            //}
        }

        public String GetLine(IntVector2 coords, int length)
        {
            // Resulting string with Pastel colors.
            String renderedString = "";

            // Each line is being grouped by char colors for the sake of optimizing drawing everything out
            // since each color tag takes a lot of hidden characters to be written out to console.
            
            // Current color in color group.
            String ColorGroupColor = "";
            // Current string composed of characters with the same color.
            String ColorGroupChars = "";

            for (int x = coords.X; x < coords.X + length; x++) {
                DrawingChar dchar = new DrawingChar(DrawingChars[coords.Y][x].Letter, DrawingChars[coords.Y][x].Color);

                //System.Diagnostics.Debug.WriteLine($"Is DrawingChar.Letter null/0 {dchar.Letter == null}/{dchar.Letter == '\0'}");
                //System.Diagnostics.Debug.WriteLine($"Is DrawingChar.Color null/empty {dchar.Color == null}/{dchar.Color == ""}");
                if (dchar.Letter == '\0') {
                    dchar.Letter = ' ';
                    //System.Diagnostics.Debug.WriteLine($"Got DrawingChar '\\0' ({dchar.Color}) on {x},{coords.Y}");
                    //System.Diagnostics.Debug.Write($"0 ");
                } else
                {
                    //System.Diagnostics.Debug.WriteLine($"Got DrawingChar '{dchar.Letter}' ({dchar.Color}) on {x},{coords.Y}");
                    //System.Diagnostics.Debug.Write($"{dchar.Letter} ");
                }

                if (ColorGroupChars.Length == 0)
                {
                    if ((dchar.Color == null) || (dchar.Color == ""))
                    {
                        renderedString += dchar.Letter;
                    }
                    else
                    {
                        ColorGroupColor = dchar.Color;
                        ColorGroupChars = dchar.Letter.ToString();
                    }
                } 
                else
                {
                    if (((dchar.Color == null) || (dchar.Color == "") || (dchar.Color == ColorGroupColor)) && (ColorGroupColor != ""))
                    {
                        ColorGroupChars += dchar.Letter;
                    }
                    else
                    {

                        if (ColorGroupChars != "" && ColorGroupColor != "" && ColorGroupChars != null && ColorGroupColor != null)
                            renderedString += ColorGroupChars.Pastel(ColorGroupColor);

                        if (dchar.Color == null || dchar.Color == "")
                            ColorGroupColor = "#FFFFFF";
                        else
                            ColorGroupColor = dchar.Color;

                        ColorGroupChars = dchar.Letter.ToString();
                    }
                }
            }

            // Finalize by adding last processed color group to rendered string.
            if (ColorGroupChars != "" && ColorGroupColor != "" && ColorGroupChars != null && ColorGroupColor != null) { 
                renderedString += ColorGroupChars.Pastel(ColorGroupColor);
            }

            return renderedString;
        }

        [Obsolete("This method is not yet implemented yet", true)]
        public static DoubleDrawingBuffer operator +(DoubleDrawingBuffer global, DoubleDrawingBuffer layer)
        {
            List<Tuple<IntVector2, int>> dirty = layer.CheckDirty();

            (DenseArray<bool> Value, uint Used) statesGlobal = global._states.LockAndModify();
            (DenseArray<bool> Value, uint Used) statesContainer = layer._states.LockAndRead();

            foreach (Tuple<IntVector2, int> line in dirty)
            {
                //Array.Copy(container.Buffer.DrawingChars[line.Item1.Y], line.Item1.X, global.DrawingChars[line.Item1.Y], line.Item1.X, line.Item2);
                //Array.Copy(statesContainer.Value.GetArray(), line.Item1.X, statesGlobal.Value.GetArray(), line.Item1.X, line.Item2);

                // FIXME: How to copy states?
                //Array.Copy(layer.states.Item1[line.Item1.Y], line.Item1.X, global.states.Item1[line.Item1.Y], line.Item1.X, line.Item2);
            }
            global._states.Unlock(statesGlobal.Used);
            layer._states.Unlock(statesContainer.Used);

            layer.AcceptDirty();

            return global;
        }

        public static DoubleDrawingBuffer operator +(DoubleDrawingBuffer global, LayeringContainer container)
        {
            List<Tuple<IntVector2, int>> dirty = container.Buffer.CheckDirty();

            (DenseArray<bool> Value, uint Used) statesGlobal = global._states.LockAndModify();
            (DenseArray<bool> Value, uint Used) statesContainer = container.Buffer._states.LockAndRead();

            foreach (Tuple<IntVector2, int> line in dirty)
            {
                Array.Copy(container.Buffer.DrawingChars[line.Item1.Y], line.Item1.X, global.DrawingChars[line.Item1.Y], line.Item1.X, line.Item2);
                Array.Copy(statesContainer.Value.GetArray(), line.Item1.X, statesGlobal.Value.GetArray(), line.Item1.X, line.Item2);

                // FIXME: How to copy states?
                //Array.Copy(layer.states.Item1[line.Item1.Y], line.Item1.X, global.states.Item1[line.Item1.Y], line.Item1.X, line.Item2);
            }
            global._states.Unlock(statesGlobal.Used);
            container.Buffer._states.Unlock(statesContainer.Used);

            container.Buffer.AcceptDirty();

            return global;
        }
    }
}
