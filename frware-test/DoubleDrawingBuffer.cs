﻿using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

        private DrawingChar[][] DrawingChars;
        private DrawingChar[][] OldDrawingChars;

        //private bool[][] _states;
        private GenericSynchronizedAlternating<bool[][]> _states;

        public DoubleDrawingBuffer(IntVector2 size) {
            this.Size = size;
            this.DrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this.OldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this._states = new GenericSynchronizedAlternating<bool[][]>(
                JaggedArrayCreator.CreateJaggedArray<bool[][]>(new int[] { size.Y, size.X }),
                JaggedArrayCreator.CreateJaggedArray<bool[][]>(new int[] { size.Y, size.X })
            );

            System.Diagnostics.Debug.WriteLine("created jagged arrays of length: dc({0},{1}) odc({2},{3}) s({4},{5})", 
                DrawingChars.Length, DrawingChars[0].Length,
                OldDrawingChars.Length, OldDrawingChars[0].Length,
                _states.UnsafeRead(1).Length, _states.UnsafeRead(1)[0].Length);
        }

        public unsafe List<Tuple<IntVector2, int>> CheckDirty() {
            List<Tuple<IntVector2, int>> dirtySectors = new List<Tuple<IntVector2, int>>(); // Coords, length

            //System.Diagnostics.Debug.WriteLine($"-> x{_states[0].Length}*y{_states.Length}");
            //System.Diagnostics.Debug.WriteLine($"-> expecting x{Size.X}*y{Size.Y}");

            (bool[][], uint) states = _states.LockAndRead();

            for (int y = 0; y < Size.Y; y++) {
                int db = -1;
                int de = -1;
                
                for (int x = 0; x < Size.X; x++) {
                    
                    if (states.Item1[y][x]) {
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

            _states.Unlock(states.Item2);

            //System.Diagnostics.Debug.WriteLine($"<- x{_states[0].Length}*y{_states.Length}"); 
            //System.Diagnostics.Debug.WriteLine($"<- expecting x{Size.X}*y{Size.Y}");

            //System.Diagnostics.Debug.WriteLine($"Checked dirty sectors successfully");
            return dirtySectors;

        }


        public void AcceptDirty() {
            (bool[][], uint) states = _states.LockAndModify();
            System.Diagnostics.Debug.WriteLine($"There should go AcceptDirty for {(states.Item2 == 1 ? "Front" : "Back")}");
            //states. = JaggedArrayCreator.CreateJaggedArray<bool[][]>(Size.Y, Size.X);
            _states.Unlock(states.Item2);
        }

        public unsafe void SetDirty(IntVector2 leftTop, IntVector2 rightBottom)
        {
            (bool[][], uint) states = _states.LockAndModify();
            IntVector2 area = rightBottom - leftTop;

            fixed (bool* a = &states.Item1[leftTop.Y][leftTop.X])
            {
                bool* b = a;
                var span = new Span<bool>(b, area.X*area.Y);
                span.Fill(true);
            }
            _states.Unlock(states.Item2);
        }

        public unsafe void SetGlobalDirty()
        {
            (bool[][], uint) states = _states.LockAndModify();
            fixed (bool* a = &states.Item1[0][0])
            {
                bool* b = a;
                var span = new Span<bool>(b, Size.X * Size.Y);
                span.Fill(true);
            }
            _states.Unlock(states.Item2);
        }

        public void DrawChar(IntVector2 coords, DrawingChar character)
        {
            (bool[][], uint) states = _states.LockAndModify();
            states.Item1[coords.Y][coords.X] = true;
            _states.Unlock(states.Item2);

            DrawingChars[coords.Y][coords.X] = character;
        }

        public void DrawLine(IntVector2 coords, DrawingChar[] line)
        {
            (bool[][], uint) states = _states.LockAndModify();
            for (int x = 0; x < line.Length; x++) {
                states.Item1[coords.Y][x + coords.X] = true;
            }
            Array.Copy(line, 0, DrawingChars[coords.Y], coords.X, line.Length);
        }

        public void DrawVLine(IntVector2 coords, DrawingChar[] line)
        {
            (bool[][], uint) states = _states.LockAndModify();
            for (int y = 0; y < line.Length; y++)
            {
                states.Item1[y + coords.Y][coords.X] = true;
                DrawingChars[y + coords.Y][coords.X] = line[y];
            }
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
                DrawingChar dchar = DrawingChars[coords.Y][x];
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

        public static DoubleDrawingBuffer operator +(DoubleDrawingBuffer global, DoubleDrawingBuffer layer)
        {
            List<Tuple<IntVector2, int>> dirty = layer.CheckDirty();

            foreach (Tuple<IntVector2, int> line in dirty) {
                Array.Copy(layer.DrawingChars[line.Item1.Y], line.Item1.X, global.DrawingChars[line.Item1.Y], line.Item1.X, line.Item2);

                // FIXME: How to copy states?
                //Array.Copy(layer.states.Item1[line.Item1.Y], line.Item1.X, global.states.Item1[line.Item1.Y], line.Item1.X, line.Item2);
            }

            layer.AcceptDirty();

            return global;
        }
    }
}
