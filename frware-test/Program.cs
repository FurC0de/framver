using Pastel;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace frware_test {
    internal class Program
    {
        #region Renderer
        public static IntVector2 Size = new IntVector2(120, 80);
        public static Renderer Renderer = new Renderer(Size);
        #endregion

        #region Clocks
        public static GameClock RenderClock = new GameClock();
        public static GameClock DataClock = new GameClock();
        #endregion

        static void Main() {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

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

           // Console.WriteLine(string.Join("", spectrum.Select(s => s.letter.Pastel(s.color))));

            Thread testDataThread = new Thread(() => testDataThreadFunc());
            testDataThread.Start();

            Thread testRefreshThread = new Thread(() => TestRefreshThreadFunc());
            testRefreshThread.Start();

            while(true)
            {
                Thread.Sleep(100);
            }
        }

        static public void testDataThreadFunc() {
            var spectrum = new (string color, string line)[]
            {
                ("#124542", "abcdefghij"),
                ("#185C58", "bcdefghija"),
                ("#1E736E", "cdefghijab"),
                ("#248A84", "defghijabc"),
                ("#20B2AA", "efghijabcd"),
                ("#3FBDB6", "fghijabcde"),
                ("#5EC8C2", "ghijabcdef"),
                ("#7DD3CE", "hijabcdefg"),
                ("#9CDEDA", "ijabcdefgh"),
                ("#BBE9E6", "jabcdefghi")
            };

            int testInt = 0;

            IntVector2 coords0 = new IntVector2(0, 0);
            IntVector2 coords1 = new IntVector2(0, 1);
            IntVector2 coords2 = new IntVector2(0, 2);
            IntVector2 coords3 = new IntVector2(0, 3);
            IntVector2 coords4 = new IntVector2(3, 4);

            DataClock.Start();

            Renderer.DrawLine(coords0, ("#AAAAAA", " ╔═ Coordinates ══════════════╗"));
            Renderer.DrawLine(coords1, ("#AAAAAA", " ║ y" + Renderer.buffer.DrawingChars.Length.ToString() + "  ;"));
            Renderer.DrawLine(coords2, ("#AAAAAA", " ║ x" + Renderer.buffer.DrawingChars[0].Length.ToString() + "  ;"));
            Renderer.DrawLine(coords3, ("#AAAAAA", " ╚════════════════════════════╝"));

            while (true)
            {
                DataClock.Step();
                
                Renderer.DrawLine(coords4, spectrum[testInt]);

                testInt = (testInt + 1) % 10;
                Thread.Sleep(60);

                // WEIRD FIX: Fixes stutter.
                Console.MoveBufferArea(0,0,0,0,0,0); 
                Console.SetCursorPosition(0, 0);

                //Console.WriteLine("DATA "+dataClock.Elapsed.TotalMilliseconds);
            }
        }

        static public void TestRefreshThreadFunc() {
            RenderClock.Start();
            while (true)
            {
                RenderClock.Step();
                Thread.Sleep(15);
                Renderer.Draw();
                //Console.SetCursorPosition(0, 0);
                //Console.WriteLine("DRAW " + renderClock.Elapsed.TotalMilliseconds);
            }
        }
    }
}