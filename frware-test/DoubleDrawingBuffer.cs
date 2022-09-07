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
            this.drawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(size.X, size.Y);
            this.oldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(size.X, size.Y);
            this.states = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(size.X, size.Y);
        }

        public List<Tuple<IntVector2, int>> checkDirty() {
            List<Tuple<IntVector2, int>> dirtySectors = new List<Tuple<IntVector2, int>>(); // Coords, length

            for (int y = 0; y < states.Length; y++) {
                int db = -1;
                int de = -1;

                for (int x = 0; x < states[y].Length; x++) {
                    if (states[y][x].changed) {
                        if (db == -1)
                            db = x;

                        if (de < x)
                            de = x;
                    }
                }

                if (db != -1)
                    dirtySectors.Add(new Tuple<int, int>(db, de));
            }

            return dirtySectors;
        }

        public void acceptDirty() {
            this.states = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(size.X, size.Y);
        }

        public void drawLine(IntVector2 coords, DrawingChar[] line) {
            for (int x = coords.X; x < line.Length; x++) {
                states[coords.Y][x].changed = true;
            }
            Array.Copy(line, 0, drawingChar[coords.Y], coords.X, line.Length);
        }
    }
}
