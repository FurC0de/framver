using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace frware_test
{
    internal static class StringExtensions
    {
        public static DrawingChar[] ToDrawingCharArray(this string s)
        {
            return s.ToDrawingCharArray("#FFFFFF");
        }

        public static DrawingChar[] ToDrawingCharArray(this string s, string color)
        {
            DrawingChar[] drawingChars = new DrawingChar[s.Length];

            for (int i = 0; i<s.Length; i++)
                drawingChars[i] = new DrawingChar(s[i], color);

            return drawingChars;
        }
    }
}
