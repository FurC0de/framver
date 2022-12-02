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
        public IntVector2 Size;
        public DrawingChar[][] DrawingChars;
        public DrawingChar[][] OldDrawingChars;
        public DrawingCharState[][] States;

        public DoubleDrawingBuffer(IntVector2 size) {
            this.Size = size;
            this.DrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this.OldDrawingChars = JaggedArrayCreator.CreateJaggedArray<DrawingChar[][]>(new int[] { size.Y, size.X });
            this.States = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(new int[] { size.Y, size.X });

            System.Diagnostics.Debug.WriteLine("created jagged arrays of length: dc({0},{1}) odc({2},{3}) s({4},{5})", 
                DrawingChars.Length, DrawingChars[0].Length,
                OldDrawingChars.Length, OldDrawingChars[0].Length,
                States.Length, States[0].Length);
        }

        public List<Tuple<IntVector2, int>> CheckDirty() {
            List<Tuple<IntVector2, int>> dirtySectors = new List<Tuple<IntVector2, int>>(); // Coords, length

            for (int y = 0; y < States.Length; y++) {
                int db = -1;
                int de = -1;

                //System.Diagnostics.Debug.WriteLine("states[y].Length is {0}", states[y].Length);

                for (int x = 0; x < States[y].Length; x++) {
                    //System.Diagnostics.Debug.Write("("+x+")");
                    if (States[y][x].Changed) {
                        //System.Diagnostics.Debug.Write(" => "+x);
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

            return dirtySectors;
        }

        public void AcceptDirty() {
            this.States = JaggedArrayCreator.CreateJaggedArray<DrawingCharState[][]>(Size.X, Size.Y);
        }

        public void DrawChar(IntVector2 coords, DrawingChar character)
        {
            States[coords.Y][coords.X].Changed = true;
            DrawingChars[coords.Y][coords.X] = character;
            //System.Diagnostics.Debug.WriteLine($"Drawing char {character.Letter} {character.Color} at {coords.X}, {coords.Y}");
        }

        public void DrawLine(IntVector2 coords, DrawingChar[] line) {
            for (int x = 0; x < line.Length; x++) {
                States[coords.Y][x + coords.X].Changed = true;
                //System.Diagnostics.Debug.WriteLine($"Setting {x+coords.X},{coords.Y} as dirty ('{line[x].Letter}')");
            }

            //System.Diagnostics.Debug.WriteLine("setting {0} chars as dirty", line.Length);
            Array.Copy(line, 0, DrawingChars[coords.Y], coords.X, line.Length);
        }

        public void DrawVLine(IntVector2 coords, DrawingChar[] line)
        {
            for (int y = 0; y < line.Length; y++)
            {
                States[y + coords.Y][coords.X].Changed = true;
                DrawingChars[y + coords.Y][coords.X] = line[y];
            }
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
                    dchar.Letter = '0';
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
    }
}
