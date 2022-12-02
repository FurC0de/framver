using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal struct DrawingChar
    {
        public char Letter;
        public string Color;

        public DrawingChar(char letter, string color)
        {
            this.Letter = letter;
            this.Color = color;
        }
    }
}
