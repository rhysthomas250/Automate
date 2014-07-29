#region License Information (GPL v3)

/*
    Copyright (C) Jaex

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ScriptingLib
{
    public static class Native
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE_LL = 14;
        public const int WHEEL_DELTA = 120;
        public const int WM_SETREDRAW = 0xB;
        public const uint SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        public static string GetForegroundWindowText()
        {
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle);
        }

        public static string GetWindowText(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                int length = GetWindowTextLength(handle);

                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);

                    if (GetWindowText(handle, sb, sb.Capacity) > 0)
                    {
                        return sb.ToString();
                    }
                }
            }

            return null;
        }
    }

    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    public struct INPUT
    {
        public InputType Type;
        public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;

        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;

        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public VirtualKeyCode wVk;
        public ushort wScan;
        public KeyboardEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point(POINT p)
        {
            return new Point(p.X, p.Y);
        }

        public static explicit operator POINT(Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Height
        {
            get
            {
                return Bottom - Top;
            }
        }

        public int Width
        {
            get
            {
                return Right - Left;
            }
        }

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
        }

        public Point Location
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        public override int GetHashCode()
        {
            return Left ^ ((Top << 13) | (Top >> 0x13))
              ^ ((Width << 0x1a) | (Width >> 6))
              ^ ((Height << 7) | (Height >> 0x19));
        }

        public static implicit operator Rectangle(RECT rect)
        {
            return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static implicit operator RECT(Rectangle rect)
        {
            return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }
}