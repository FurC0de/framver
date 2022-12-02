using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace frware_test
{
    internal class ConsoleUtilities
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        public static void Maximize()
        {
            IntPtr window = GetConsoleWindow();
            ShowWindow(window, 3); //SW_MAXIMIZE = 3
        }

        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, bottom, right;
        }

        private static readonly string WINDOW_NAME = "w-framver-testbuild-00411";  //name of the window
        private const int GWL_STYLE = -16;              //hex constant for style changing
        private const int WS_BORDER = 0x00800000;       //window with border
        private const int WS_CAPTION = 0x00C00000;      //window with a title bar
        private const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
        private const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox

        static void makeBorderless()
        {
            // Get the handle of self
            IntPtr window = GetConsoleWindow();
            Console.WriteLine(window);

            RECT rect;
            // Get the rectangle of self (Size)
            GetWindowRect(window, out rect);

            Console.WriteLine($"window rect {rect.left},{rect.top} {rect.right},{rect.bottom}");

            RECT desktopRect;
            // Get the handle of the desktop
            IntPtr HWND_DESKTOP = GetDesktopWindow();
            GetWindowRect(HWND_DESKTOP, out desktopRect);
            // Attempt to get the location of self compared to desktop
            MapWindowPoints(HWND_DESKTOP, window, ref rect, 2);
            // update self
            SetWindowLong(window, GWL_STYLE, WS_SYSMENU);
            // rect.left rect.top should work but they're returning negative values for me. I probably messed up
            SetWindowPos(window, -2, 0, 0, desktopRect.bottom, desktopRect.right, 0x0040);
            DrawMenuBar(window);
        }

        public static void Borderless()
        {
            Console.Title = WINDOW_NAME;
            Console.WriteLine("Enabling borderless...");
            Thread.Sleep(1000);
            makeBorderless();
            Console.WriteLine("Enabled: borderless");
            Console.ReadLine();
        }
    }
}