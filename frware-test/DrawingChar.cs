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
        string letter = "";
        string color = "";

        public DrawingChar(string letter, string color)
        {
            this.letter = letter;
            this.color = color;
        }
    }
}
