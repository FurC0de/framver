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
        public char letter;
        public string color;

        public DrawingChar(char letter, string color)
        {
            this.letter = letter;
            this.color = color;
        }
    }
}
