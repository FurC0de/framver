// See https://aka.ms/new-console-template for more information

using Pastel;
using System.Drawing;

namespace frware_test {
    internal class Program
    {
        public static IntVector2 size = new IntVector2(120, 80);
        public static Renderer renderer = new Renderer(size);
        static void Main() {

            var spectrum = new (string color, string letter)[]
            {
                ("#124542", "a"),
                ("#185C58", "b"),
                ("#1E736E", "c"),
                ("#248A84", "d"),
                ("#20B2AA", "e"),
                ("#3FBDB6", "f"),
                ("#5EC8C2", "g"),
                ("#7DD3CE", "i"),
                ("#9CDEDA", "j"),
                ("#BBE9E6", "k")
            };

            Console.WriteLine(string.Join("", spectrum.Select(s => s.letter.Pastel(s.color))));
        }
    }
}