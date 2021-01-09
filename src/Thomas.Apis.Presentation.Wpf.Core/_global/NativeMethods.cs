using System;
using System.Runtime.InteropServices;
using System.Windows;

public static class NativeMethods
{
    #region Native Methods

    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static Rectangle GetClientRect(IntPtr hWnd)
    {
        Rectangle result;
        GetClientRect(hWnd, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Gets the window position UI flag "NoZOrder" 
    /// </summary>
    public const int SwpNoZOrder = 0x0004;


    /// <summary>
    /// Gets the window position UI flag "NoActivate" 
    /// </summary>
    public const int SwpNoactivate = 0x0010;

    /// <summary>
    /// Gets windows property id for style 
    /// </summary>
    public const int GwlStyle = -16;

    /// <summary>
    /// Gets the window position UI flag "Caption" 
    /// </summary>
    public const int WsCaption = 0x00C00000;

    /// <summary>
    /// Gets the window position UI flag "ThickFrame" 
    /// </summary>
    public const int WsThickframe = 0x00040000;

    /// <summary>
    /// Gets the top window pointer from the given point.
    /// </summary>
    /// <param name="lpPoint">The absolute point.</param>
    /// <returns>The window point.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr WindowFromPoint(Point lpPoint);

    /// <summary>
    /// Sets some window porperties.
    /// </summary>
    /// <param name="hWnd">The pointer of the window.</param>
    /// <param name="nIndex">The id of the property</param>
    /// <param name="dwNewLong">The value of the property.</param>
    /// <returns>The result/error code.</returns>
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    /// <summary>
    /// Gets some window properties.
    /// </summary>
    /// <param name="hWnd">The pointer of the window.</param>
    /// <param name="nIndex">The id of the property</param>
    /// <returns>The value of the property.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    /// <summary>
    /// Changes the parent of the window.
    /// </summary>
    /// <param name="hWnd">The pointer of the window.</param>
    /// <param name="hWndParent">The pointer of the new parent.</param>
    /// <returns></returns>
    [DllImport("user32")]
    public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="hWnd">The pointer of the window.</param>
    /// <param name="hWndInsertAfter"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <param name="uFlags"></param>
    /// <returns></returns>
    [DllImport("user32")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy,
        uint uFlags);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="nCmdShow"></param>
    /// <returns></returns>
    [DllImport("User32")]
    public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="lpRect"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        /// <summary>
        /// 
        /// </summary>
        public int Left; // x position of upper-left corner

        /// <summary>
        /// 
        /// </summary>
        public int Top; // y position of upper-left corner

        /// <summary>
        /// 
        /// </summary>
        public int Right; // x position of lower-right corner

        /// <summary>
        /// 
        /// </summary>
        public int Bottom; // y position of lower-right corner
    }


    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Gets active process.
    /// </summary>
    /// <returns></returns>
    public static int GetActiveProcessId()
    {
        var hwnd = GetForegroundWindow();
        uint pid;
        GetWindowThreadProcessId(hwnd, out pid);
        return (int)pid;
    }

    /// <summary>
    /// An global event that is fired when the foreground process changes.
    /// </summary>
    public static event Action<IntPtr> ForegroundProcessChanged
    {
        add
        {
            if (ForegroundProcessChangedEvent == null)
            {
                ForegroundProcessChangedNativeEventHandler = OnForegroundProcessChanged;

                ForegroundProcessChangedNativeEventHook = SetWinEventHook(
                    EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    ForegroundProcessChangedNativeEventHandler, 0, 0, WINEVENT_OUTOFCONTEXT);

            }
            ForegroundProcessChangedEvent += value;
        }
        remove
        {
            ForegroundProcessChangedEvent -= value;
            if (ForegroundProcessChangedEvent == null)
            {
                ForegroundProcessChangedNativeEventHandler = null;
                ForegroundProcessChangedNativeEventHook = IntPtr.Zero;
            }
        }
    }

    private static event Action<IntPtr> ForegroundProcessChangedEvent;
    private static IntPtr ForegroundProcessChangedNativeEventHook { get; set; }
    private static WinEventDelegate ForegroundProcessChangedNativeEventHandler { get; set; }


    private delegate void WinEventDelegate(
        IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
        uint dwmsEventTime);

    [DllImport("user32.dll")]
    static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    private const uint WINEVENT_OUTOFCONTEXT = 0;
    private const uint EVENT_SYSTEM_FOREGROUND = 3;

    private static void OnForegroundProcessChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        ForegroundProcessChangedEvent?.Invoke(hwnd);
    }


}