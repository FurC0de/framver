using Pastel;
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
        public static GameClock RenderClock = new GameClock(true);
        public static GameClock DataClock = new GameClock(false);
        public static GameClock InputClock = new GameClock(false);
        #endregion

        static void Main() {
            Thread.Sleep(5000);
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (OperatingSystem.IsWindows()) {
#pragma warning disable CA1416
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(Size.X, Size.Y);
                Console.SetWindowSize(120, 60);
                //WindowsConsoleUtilities.MakeBorderless();
                WindowsConsoleUtilities.DisableQuickEdit();
#pragma warning restore CA1416
            }

            Console.Clear();

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
                        //WindowsConsoleUtilities.DisableBorderless();
                        WindowsConsoleUtilities.EnableQuickEdit();

                        Console.Clear();
                        Console.SetCursorPosition(0, 0);

                        Console.WriteLine($"Data latency");
                        Console.WriteLine($"    max : {DataClock.ElapsedMax.TotalMilliseconds,7:0.00} ms");

                        // TODO: Move calculations to GameClock.Stop()
                        RenderClock.Frametimes.Sort((a, b) => b.CompareTo(a));

                        Console.WriteLine($"Render latency ({RenderClock.Frametimes.Count} samples)");
                        Console.WriteLine($"    max : {RenderClock.ElapsedMax.TotalMilliseconds,7:0.00} ms");
                        Console.WriteLine($"    avg : {RenderClock.Frametimes.Average(),7:0.00} ms");
                        Console.WriteLine($"    10% : {RenderClock.Frametimes[RenderClock.Frametimes.Count / 10],7:0.00} ms");
                        Console.WriteLine($"     1% : {RenderClock.Frametimes[RenderClock.Frametimes.Count / 100],7:0.00} ms");
                        Console.WriteLine($"   0.1% : {RenderClock.Frametimes[RenderClock.Frametimes.Count / 1000],7:0.00} ms");
                        Console.WriteLine($"     hi : {RenderClock.Frametimes[0],7:0.00} ms");
                        Console.WriteLine($"    low : {RenderClock.Frametimes[^1],7:0.00} ms");

                        Environment.Exit(0);
                    }
                }
                Thread.Sleep(1);
            }
        }

        static public void testDataThreadFunc(CancellationToken cancelToken) {

            LayeringContainer container_A = new LayeringContainer(Size);
            Window window_A = new Window(new IntVector2(30,10), new IntVector2(1, 12));
            Renderer.AddLayeringContainer(container_A);
            container_A.Buffer.DrawLine(new IntVector2(1, 10), ($"I am a container with a window [Buffer_Direct_Access]").ToDrawingCharArray());
            container_A.AddWindow(window_A);
            container_A.UpdateBuffer();

            Renderer.Buffer.SetGlobalDirty();

            DataClock.Start();

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    DataClock.Stop();
                    return;
                }

                DataClock.Step();
                Thread.Sleep(15);

                #region DebugLines
                Renderer.DrawLine(new IntVector2(1, 1), ($"Data   : {DataClock.Elapsed.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(19, 1), ($"Max : {DataClock.ElapsedMax.TotalMilliseconds}").ToDrawingCharArray());

                Renderer.DrawLine(new IntVector2(1, 2), ($"Render : {RenderClock.Elapsed.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(19, 2), ($"Max : {RenderClock.ElapsedMax.TotalMilliseconds}").ToDrawingCharArray());
                Renderer.DrawLine(new IntVector2(37, 2), ($"Delay : {Math.Clamp(60 - ((int)RenderClock.Elapsed.TotalMilliseconds), 0, 30)}").ToDrawingCharArray());

                Renderer.DrawLine(new IntVector2(1, 4), ($"Containers ({Renderer.LayeringContainers.Count}):").ToDrawingCharArray());
                for (int i = 0; i < Renderer.LayeringContainers.Count; i++)
                {
                    Renderer.DrawLine(new IntVector2(3, 5+i), ($"container_{Renderer.LayeringContainers[i].ContainerUID}").ToDrawingCharArray("#BDBDBD"));
                }
                #endregion

                #region Container_A
                #endregion

                // WEIRD FIX: Fixes stutter.
                // Console.MoveBufferArea(0,0,0,0,0,0); 
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

                Thread.Sleep(Math.Clamp(60 - ((int)RenderClock.Elapsed.TotalMilliseconds), 0, 30));
                RenderClock.Step();
            }
        }
    }
}