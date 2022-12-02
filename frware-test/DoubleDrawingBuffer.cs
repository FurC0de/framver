using Pastel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class DoubleDrawingBuffer
    {
        public IntVector2 size;
        public DrawingChar[][] drawingChars;
        public DrawingChar[][] oldDrawingChars;
        public DrawingCharState[][] states;

        public DoubleDrawingBuffer(IntVector2 size) {
            this.size = size;
            this.drawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.X, size.Y } );
            this.oldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.X, size.Y });
            this.states = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(new int[] { size.X, size.Y });

            System.Diagnostics.Debug.WriteLine("created jagged arrays of length: dc({0},{1}) odc({2},{3}) s({4},{5})", 
                drawingChars.Length, drawingChars[0].Length,
                oldDrawingChars.Length, oldDrawingChars[0].Length,
                states.Length, states[0].Length);
        }

        public List<Tuple<IntVector2, int>> checkDirty() {
            List<Tuple<IntVector2, int>> dirtySectors = new List<Tuple<IntVector2, int>>(); // Coords, length

            for (int y = 0; y < states.Length; y++) {
                int db = -1;
                int de = -1;

                //System.Diagnostics.Debug.WriteLine("states[y].Length is {0}", states[y].Length);

                for (int x = 0; x < states[y].Length; x++) {
                    //System.Diagnostics.Debug.Write("("+x+")");
                    if (states[y][x].changed) {
                        //System.Diagnostics.Debug.Write(" => "+x);
                        if (db == -1)
                            db = x;

                        if (de < x)
                            de = x;
                    }
                }

                if (db != -1) {
                    dirtySectors.Add(new Tuple<IntVector2, int>(new IntVector2(db, y), de - db));
                    //System.Diagnostics.Debug.WriteLine("adding dirty sector: from {0} to {1} of len {2}, on coords ({3} {4})", db, de, de - db, db, y);
                }
            }

            return dirtySectors;
        }

        public void acceptDirty() {
            this.states = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(size.X, size.Y);
        }

        public void drawLine(IntVector2 coords, DrawingChar[] line) {
            for (int x = coords.X; x < line.Length+line.Length; x++) {
                states[coords.Y][x].changed = true;
            }

            //System.Diagnostics.Debug.WriteLine("setting {0} chars as dirty", line.Length);

            Array.Copy(line, 0, drawingChars[coords.Y], coords.X, line.Length);
        }

        public String getLine(IntVector2 coords, int length)
        {
            String renderedString = "";

            String solidCLR = "";
            String solidCCA = "";

            for (int x = coords.X; x < coords.X + length; x++) {
                DrawingChar dchar = drawingChars[coords.Y][x];

                if (dchar.color == solidCLR) {
                    solidCCA += dchar.letter;
                } else {
                    if (solidCCA != "" && solidCLR != "" && solidCCA != null && solidCLR != null) {
                        renderedString += solidCCA.Pastel(solidCLR);
                    }

                    solidCLR = dchar.color;
                    solidCCA = "";
                    solidCCA += dchar.letter;
                }
            }

            if (solidCCA != "" && solidCLR != "" && solidCCA != null && solidCLR != null)
                renderedString += solidCCA.Pastel(solidCLR);

            return renderedString;
        }
    }
}
