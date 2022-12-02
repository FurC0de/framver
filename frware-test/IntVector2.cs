using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal struct IntVector2
    {
        public int X;
        public int Y;
        
        public IntVector2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.X + b.X, a.Y + b.Y);

        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.X - b.X, a.Y - b.Y);

        public static IntVector2 operator +(IntVector2 a, Tuple<int, int> t) => new IntVector2(a.X + t.Item1, a.Y + t.Item2);

        public static IntVector2 operator -(IntVector2 a, Tuple<int, int> t) => new IntVector2(a.X - t.Item1, a.Y - t.Item2);

        public static IntVector2 operator +(IntVector2 a, (int, int) t) => new IntVector2(a.X + t.Item1, a.Y + t.Item2);

        public static IntVector2 operator -(IntVector2 a, (int, int) t) => new IntVector2(a.X - t.Item1, a.Y - t.Item2);
    }
}
