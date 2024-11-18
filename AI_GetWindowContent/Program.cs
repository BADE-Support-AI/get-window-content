namespace AI_GetWindowContent;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    // WinAPI-Functions
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_CLOSE = 0x0010;

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    static void Main()
    {
        string windowTitle = "TwinCAT System"; // Window title must be known for this to work

        try
        {
            // Search for window
            IntPtr mainWindowHandle = FindWindow(null, windowTitle);
            if (mainWindowHandle == IntPtr.Zero)
            {
                Console.WriteLine("Window could not be found.");
                return;
            }

            // Search in child-windows
            List<string> windowContents = new List<string>();
            EnumChildWindows(mainWindowHandle, (hWnd, lParam) =>
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    windowContents.Add(sb.ToString());
                }
                return true; // Continue searching
            }, IntPtr.Zero);

            // Print results
            Console.WriteLine("Window content:");
            foreach (string content in windowContents)
            {
                Console.WriteLine(content);
            }

            // Close window
            if (PostMessage(mainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero))
                Console.WriteLine("Window closed.");
            else
                Console.WriteLine("Failed to close window.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
