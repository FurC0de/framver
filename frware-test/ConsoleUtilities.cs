using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace frware_test
{
    internal static class WindowsConsoleUtilities
    {
        #region SWP_Flags

        [Flags]
        public enum SWP_Flags : uint
        {
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            SWP_DRAWFRAME = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>Displays the window.</summary>
            SWP_SHOWWINDOW = 0x0040,
            /// <summary>Hides the window.</summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            SWP_NOREPOSITION = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            SWP_NOSENDCHANGING = 0x0400,
            /// <summary>Internal use.</summary>
            SWP_NOCLIENTSIZE = 0x0800,
            /// <summary>Internal use.</summary>
            SWP_NOCLIENTMOVE = 0x1000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            SWP_ASYNCWINDOWPOS = 0x4000
        }

        #endregion

        #region WinStyles

        [Flags]
        public enum WinStyles : uint
        {
            WS_BORDER = 0x00800000,                     //The window has a thin-line border.
            WS_CAPTION = 0x00C00000,                    //The window has a title bar (includes the WS_BORDER style).
            WS_CHILD = 0x40000000,                      //The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
            WS_CHILDWINDOW = 0x40000000,                //Same as the WS_CHILD style.
            WS_CLIPCHILDREN = 0x02000000,               //Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
            WS_CLIPSIBLINGS = 0x04000000,               //Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            WS_DISABLED = 0x08000000,                   //The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.
            WS_DLGFRAME = 0x00400000,                   //The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
            WS_GROUP = 0x00020000,                      //The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style. The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
                                                        //You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            WS_HSCROLL = 0x00100000,                    //The window has a horizontal scroll bar.
            WS_ICONIC = 0x20000000,                     //The window is initially minimized. Same as the WS_MINIMIZE style.
            WS_MAXIMIZE = 0x01000000,                   //The window is initially maximized.
            WS_MAXIMIZEBOX = 0x00010000,                //The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            WS_MINIMIZE = 0x20000000,                   //The window is initially minimized. Same as the WS_ICONIC style.
            WS_MINIMIZEBOX = 0x00020000,                //The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
            WS_OVERLAPPED = 0x00000000,                 //The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED |       //The window is an overlapped window. Same as the WS_TILEDWINDOW style.
                                  WS_CAPTION |
                                  WS_SYSMENU |
                                  WS_THICKFRAME |
                                  WS_MINIMIZEBOX |
                                  WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000,                      //The windows is a pop-up window. This style cannot be used with the WS_CHILD style.
            WS_POPUPWINDOW = WS_POPUP |                 //The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
                             WS_BORDER |
                             WS_SYSMENU,
            WS_SIZEBOX = 0x00040000,                    //The window has a sizing border. Same as the WS_THICKFRAME style.
            WS_SYSMENU = 0x00080000,                    //The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
            WS_TABSTOP = 0x00010000,                    //The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
                                                        //You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function. For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            WS_THICKFRAME = 0x00040000,                 //The window has a sizing border. Same as the WS_SIZEBOX style.
            WS_TILED = 0x00000000,                      //The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.
            WS_TILEDWINDOW = WS_OVERLAPPED |            //The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.
                             WS_CAPTION |
                             WS_SYSMENU |
                             WS_THICKFRAME |
                             WS_MINIMIZEBOX |
                             WS_MAXIMIZEBOX,
            WS_VISIBLE = 0x10000000,                   //The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.
            WS_VSCROLL = 0x00200000,                   //The window has a vertical scroll bar.
        }

        #endregion

        #region GWL_Flags

        public enum GWL_Flags : int
        {
            GWL_USERDATA = -21,
            GWL_EXSTYLE = -20,
            GWL_STYLE = -16,
            GWL_ID = -12,
            GWLP_HWNDPARENT = -8,
            GWLP_HINSTANCE = -6,
            GWL_WNDPROC = -4,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4,
            DWLP_USER = 0x8
        }

        #endregion

        #region user32

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        #endregion

        #region kernel32

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        #endregion

        #region RECT
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, bottom, right;
        }
        #endregion

        private static readonly string WINDOW_NAME = "w-framver-testbuild-00411";  //name of the window

        public static void Maximize()
        {
            IntPtr window = GetConsoleWindow();
            ShowWindow(window, 3); //SW_MAXIMIZE = 3
        }

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
            SetWindowLong(window, (int)GWL_Flags.GWL_STYLE, (int)WinStyles.WS_SYSMENU);
            // rect.left rect.top should work but they're returning negative values for me. I probably messed up
            SetWindowPos(window, -2, 0, 0, desktopRect.bottom, desktopRect.right, 0x0040);
            DrawMenuBar(window);
        }

        static void disableBorderless()
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
            SetWindowLong(window, (int)GWL_Flags.GWL_STYLE, (int)WinStyles.WS_TILEDWINDOW);
            // rect.left rect.top should work but they're returning negative values for me. I probably messed up
            SetWindowPos(window, -2, 100, 100, 500, 300, 0x0040);
            DrawMenuBar(window);
        }

        public static void MakeBorderless()
        {
            Console.Title = WINDOW_NAME;
            Console.WriteLine("Enabling borderless...");
            Thread.Sleep(1000);
            makeBorderless();
            Console.WriteLine("Enabled: borderless");
            Thread.Sleep(200);
        }

        public static void DisableBorderless()
        {
            Console.Title = WINDOW_NAME;
            Console.WriteLine("Disabling borderless...");
            Thread.Sleep(1000);
            disableBorderless();
            Console.WriteLine("Disabled: borderless");
            Thread.Sleep(200);
        }


        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        internal static bool DisableQuickEdit()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                Console.WriteLine("ERROR: Unable to get console mode.");
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                Console.WriteLine("ERROR: Unable to set console mode.");
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }

        internal static bool EnableQuickEdit()
        {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                Console.WriteLine("ERROR: Unable to get console mode.");
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode |= ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                Console.WriteLine("ERROR: Unable to set console mode.");
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }
}