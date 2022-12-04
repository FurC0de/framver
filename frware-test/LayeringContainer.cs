using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    // TODO: Rename to DrawingContainer (probably)
    internal class LayeringContainer
    {
        private static Random s_Random = new Random();

        public readonly string ContainerUID;

        public DoubleDrawingBuffer Buffer;

        public bool Visible = true;

        public bool Updated = false;

        const int WindowsAmount = 8;
        const int WindowsCapacity = 11;

        private ConcurrentDictionary<string, Window> Windows;

        // TODO: Implement container thread
        public Thread Thread;

        public LayeringContainer(IntVector2 size)
        {
            ContainerUID = s_Random.Next().ToString("x");
            Buffer = new DoubleDrawingBuffer(size);
            Windows = new ConcurrentDictionary<string, Window>(Environment.ProcessorCount * 2, WindowsCapacity);
        }

        public string AddWindow(Window window)
        {
            string WindowUID = $"{ContainerUID}-{s_Random.Next().ToString("x")}";
            Windows.TryAdd(WindowUID, window);
            return WindowUID;
        }

        public Window GetWindow(string WindowUID)
        {
            return Windows[WindowUID];
        }

        public void UpdateBuffer()
        {
            foreach (Window window in Windows.Values)
            {
                Buffer.DrawWindow(window);
            }
        }
    }
}
