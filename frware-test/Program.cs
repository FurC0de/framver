﻿using Pastel;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace frware_test {
    internal class Program
    {
        #region Renderer
        public static IntVector2 Size = new IntVector2(120, 60);
        public static Renderer Renderer = new Renderer(Size);
        #endregion

        #region Clocks
        public static GameClock RenderClock = new GameClock();
        public static GameClock DataClock = new GameClock();
        public static GameClock InputClock = new GameClock();
        #endregion

        static void Main() {
            Thread.Sleep(5000);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (OperatingSystem.IsWindows()) {
#pragma warning disable CA1416
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(Size.X, Size.Y);
                WindowsConsoleUtilities.MakeBorderless();
                WindowsConsoleUtilities.DisableQuickEdit();
#pragma warning restore CA1416
            }

            Console.Clear();

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

            Renderer.Buffer.SetGlobalDirty();

            // Console.WriteLine(string.Join("", spectrum.Select(s => s.letter.Pastel(s.color))));
            CancellationTokenSource cancelSource = new CancellationTokenSource();

            Thread testDataThread = new Thread(() => testDataThreadFunc(cancelSource.Token));
            testDataThread.Start();

            Thread testRefreshThread = new Thread(() => TestRefreshThreadFunc(cancelSource.Token));
            testRefreshThread.Start();


            while (true)
            {
                if (NativeKeyboard.IsKeyDown(KeyCode.Escape))
                {
                    if (testDataThread.ThreadState == ThreadState.Running || testRefreshThread.ThreadState == ThreadState.Running)
                    {
                        if (!cancelSource.IsCancellationRequested)
                            cancelSource.Cancel();
                    }
                    else
                    {
                        WindowsConsoleUtilities.DisableBorderless();
                        WindowsConsoleUtilities.EnableQuickEdit();

                        Console.WriteLine($"Data latency max    : {DataClock.ElapsedMax.TotalMilliseconds}");
                        Console.WriteLine($"Render latencty max : {RenderClock.ElapsedMax.TotalMilliseconds}");

                        Environment.Exit(0);
                    }
                }
                Thread.Sleep(1);
            }
        }

        static public void testDataThreadFunc(CancellationToken cancelToken) {
            /*
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

            

            Renderer.DrawLine(coords0, ("#AAAAAA", " ╔═ Coordinates ══════════════╗"));
            Renderer.DrawLine(coords1, ("#AAAAAA", " ║ y" + Renderer.Buffer.DrawingChars.Length.ToString() + "  ;"));
            Renderer.DrawLine(coords2, ("#AAAAAA", " ║ x" + Renderer.Buffer.DrawingChars[0].Length.ToString() + "  ;"));
            Renderer.DrawLine(coords3, ("#AAAAAA", " ╚════════════════════════════╝"));

            Window testWindow1 = new Window(new IntVector2(22,8), new IntVector2(3, 6));
            testWindow1.Init();
            testWindow1.SetTitle("Title_A");
            Renderer.DrawWindow(testWindow1);

            Window testWindow2 = new Window(new IntVector2(110, 50), new IntVector2(10, 10));
            testWindow2.Init();
            testWindow2.SetTitle("Title_B");
            Renderer.DrawWindow(testWindow2);

            Window testWindow3 = new Window(new IntVector2(7, 8), new IntVector2(3, 26));
            testWindow3.Init();
            testWindow3.SetTitle("Title");
            Renderer.DrawWindow(testWindow3);
            */

            Renderer.AddLayeringContainer(new LayeringContainer(Size));

            DataClock.Start();

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    DataClock.Stop();
                    return;
                }

                DataClock.Step();
                Thread.Sleep(100);

                #region DebugLines
                Renderer.DrawLine(new IntVector2(1, 1), ($"Data   : {DataClock.Elapsed.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(19, 1), ($"Max : {DataClock.ElapsedMax.TotalMilliseconds}").ToDrawingCharArray());

                Renderer.DrawLine(new IntVector2(1, 2), ($"Render : {RenderClock.Elapsed.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(19, 2), ($"Max : {RenderClock.ElapsedMax.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(35, 2), ($"Delay : {Math.Clamp(60 - ((int)RenderClock.Elapsed.TotalMilliseconds), 0, 30)}").ToDrawingCharArray());

                Renderer.DrawLine(new IntVector2(1, 4), ($"Containers ({Renderer.LayeringContainers.Count}):").ToDrawingCharArray());
                for (int i = 0; i < Renderer.LayeringContainers.Count; i++)
                {
                    Renderer.DrawLine(new IntVector2(3, 5+i), ($"container_{Renderer.LayeringContainers[i].ContainerUID}").ToDrawingCharArray("#BDBDBD"));
                }
                #endregion

                //Renderer.DrawLine(coords4, spectrum[testInt]);
                //testInt = (testInt + 1) % 10;

                // WEIRD FIX: Fixes stutter.
                Console.MoveBufferArea(0,0,0,0,0,0); 

                //Console.SetCursorPosition(80, 50);
                //Console.WriteLine("DATA "+dataClock.Elapsed.TotalMilliseconds);
            }
        }

        static public void TestRefreshThreadFunc(CancellationToken cancelToken) {
            RenderClock.Start();
            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    RenderClock.Stop();
                    return;
                }

                Renderer.Draw();

                RenderClock.Step();
                Thread.Sleep(Math.Clamp(60-((int)RenderClock.Elapsed.TotalMilliseconds), 0, 30));
            }
        }
    }
}