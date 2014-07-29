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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ScriptingLib
{
    public static class Helpers
    {
        public static Point GetCursorPosition()
        {
            POINT currentMousePoint;

            if (Native.GetCursorPos(out currentMousePoint))
            {
                return (Point)currentMousePoint;
            }

            return Point.Empty;
        }

        public static void SetCursorPosition(int x, int y)
        {
            Native.SetCursorPos(x, y);
        }

        public static void SetCursorPosition(Point position)
        {
            SetCursorPosition(position.X, position.Y);
        }

        public static Color GetPixelColor()
        {
            return GetPixelColor(GetCursorPosition());
        }

        public static Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = Native.GetDC(IntPtr.Zero);
            uint pixel = Native.GetPixel(hdc, x, y);
            Native.ReleaseDC(IntPtr.Zero, hdc);
            return Color.FromArgb((int)(pixel & 0x000000FF), (int)(pixel & 0x0000FF00) >> 8, (int)(pixel & 0x00FF0000) >> 16);
        }

        public static Color GetPixelColor(Point position)
        {
            return GetPixelColor(position.X, position.Y);
        }

        public static bool CheckPixelColor(int x, int y, Color color)
        {
            Color targetColor = GetPixelColor(x, y);

            return targetColor.R == color.R && targetColor.G == color.G && targetColor.B == color.B;
        }

        public static bool CheckPixelColor(int x, int y, Color color, byte variation)
        {
            Color targetColor = GetPixelColor(x, y);

            return targetColor.R.IsBetween(color.R - variation, color.R + variation) &&
                targetColor.G.IsBetween(color.G - variation, color.G + variation) &&
                targetColor.B.IsBetween(color.B - variation, color.B + variation);
        }

        public static bool WaitWhile(Func<bool> check, int interval, int timeout = -1)
        {
            Stopwatch timer = Stopwatch.StartNew();

            while (check())
            {
                if (timeout >= 0 && timer.ElapsedMilliseconds >= timeout)
                {
                    return false;
                }

                Thread.Sleep(interval);
            }

            return true;
        }

        public static void WaitWhileAsync(Func<bool> check, int interval, int timeout, Action onSuccess)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (sender, e) => e.Result = WaitWhile(check, interval, timeout);
            bw.RunWorkerCompleted += (sender, e) => { if ((bool)e.Result) onSuccess(); };
            bw.RunWorkerAsync();
        }
    }
}