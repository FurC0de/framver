using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    internal class Container
    {
        private static Random s_Random = new Random();

        public readonly string ContainerUID;

        public DoubleDrawingBuffer Buffer;

        public bool Visible = true;

        public bool Updated = false;

        public Dictionary<string, Window> Windows;

        public Thread Thread;

        public Container(IntVector2 size)
        {
            ContainerUID = s_Random.Next().ToString("x");
            Buffer = new DoubleDrawingBuffer(size);
        }

        public string AddWindow(Window window)
        {
            string WindowUID = $"{ContainerUID}-{s_Random.Next().ToString("x")}";
            Windows.Add(WindowUID, window);

            return WindowUID;
        }

        public Window GetWindow(string WindowUID)
        {
            return Windows[WindowUID];
        }
    }
}
