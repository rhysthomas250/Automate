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
using System.Windows.Forms;

namespace ScriptingLib
{
    public static class Extensions
    {
        public static bool IsBetween(this byte num, int min, int max)
        {
            return num >= min && num <= max;
        }

        public static void BeginUpdate(this RichTextBox rtb)
        {
            Native.SendMessage(rtb.Handle, Native.WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public static void EndUpdate(this RichTextBox rtb)
        {
            Native.SendMessage(rtb.Handle, Native.WM_SETREDRAW, 1, IntPtr.Zero);
            rtb.Invalidate();
        }
    }
}