using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace ObjectManager.Infrastructure
{
    public class KeyboardCommandDispatcher
    {
        [DllImport("USER32.DLL")]
        private static extern int PostMessage(IntPtr hwnd, uint msg, int character, uint lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        #region VirtualKeyCodes Enum
        public enum VirtualKeyCodes
        {
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04,
            VK_XBUTTON1 = 0x05,
            VK_XBUTTON2 = 0x06,
            VK_BACK = 0x08,
            VK_TAB = 0x09,
            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_ALT = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPSLOCK = 0x14,
            VK_ESCAPE = 0x1B,
            VK_SPACE = 0x20,
            VK_PAGEUP = 0x21,
            VK_PAGEDOWN = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_PRINTSCREEN = 0x2A,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,
            VK_0 = 0x30,
            VK_1 = 0x31,
            VK_2 = 0x32,
            VK_3 = 0x33,
            VK_4 = 0x34,
            VK_5 = 0x35,
            VK_6 = 0x36,
            VK_7 = 0x37,
            VK_8 = 0x38,
            VK_9 = 0x39,
            VK_A = 0x41,
            VK_B = 0x42,
            VK_C = 0x43,
            VK_D = 0x44,
            VK_E = 0x45,
            VK_F = 0x46,
            VK_G = 0x47,
            VK_H = 0x48,
            VK_I = 0x49,
            VK_J = 0x4A,
            VK_K = 0x4B,
            VK_L = 0x4C,
            VK_M = 0x4D,
            VK_N = 0x4E,
            VK_O = 0x4F,
            VK_P = 0x50,
            VK_Q = 0x51,
            VK_R = 0x52,
            VK_S = 0x53,
            VK_T = 0x54,
            VK_U = 0x55,
            VK_V = 0x56,
            VK_W = 0x57,
            VK_X = 0x58,
            VK_Y = 0x59,
            VK_Z = 0x5A,
            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPERATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_NUMLOCK = 0x90,
            VK_SCROLLLOCK = 0x91,
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,
            VK_PLUS = 0xBB,
            VK_MINUS = 0xBD,
            VK_TILDE = 0xC0
        }
        #endregion

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;
        private const uint WM_LBUTTONDOWN = 0x201;
        private const uint WM_LBUTTONUP = 0x202;
        private const uint WM_RBUTTONDOWN = 0x204;
        private const uint WM_RBUTTONUP = 0x205;
        private const uint WM_MOVEMOUSE = 0x0200;

        [Flags]
        private enum MouseFlags
        {
            MK_CONTROL = 0x0008,
            MK_LBUTTON = 0x0001,
            MK_MBUTTON = 0x0010,
            MK_RBUTTON = 0x0002,
            MK_SHIFT = 0x0004,
            MK_XBUTTON1 = 0x0020,
            MK_XBUTTON2 = 0x0040
        }

        public void SendClick(Process process, Point point)
        {
            var processWindowHandle = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, null);
            var lparam = CreateMouseParam(point);
            PostMessage(processWindowHandle, WM_RBUTTONDOWN, 0x01, lparam);
            PostMessage(processWindowHandle, WM_RBUTTONUP, 0x01, lparam);
        }

        public void MoveMouse(Process process, Point point)
        {
            var lparam = CreateMouseParam(point);
            var flags = MouseFlags.MK_LBUTTON | MouseFlags.MK_SHIFT;

            PostMessage(process.MainWindowHandle, WM_MOVEMOUSE, 0, lparam);
         //   PostMessage(process.MainWindowHandle, 0x0020, 0x00050038, 0x02000001);
        }

        public void SendShiftClick(Process process, Point point)
        {
            //var processWindowHandle = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, null);
            //if ((uint)processWindowHandle == 0) processWindowHandle = process.MainWindowHandle;

            var lparam = CreateMouseParam(point);
            var flags = MouseFlags.MK_LBUTTON | MouseFlags.MK_SHIFT;

            PostMessage(process.MainWindowHandle, WM_KEYDOWN, (int)VirtualKeyCodes.VK_SHIFT, 0x002A0001);
            Thread.Sleep(100);
            PostMessage(process.MainWindowHandle, WM_MOVEMOUSE, 0, lparam);
            PostMessage(process.MainWindowHandle, 0x0020, 0x00050038, 0x02000001);
            Thread.Sleep(100);
            PostMessage(process.MainWindowHandle, WM_RBUTTONDOWN, (int)flags, lparam);
            Thread.Sleep(100);
            PostMessage(process.MainWindowHandle, WM_RBUTTONUP, (int)flags, lparam);
            Thread.Sleep(100);
            PostMessage(process.MainWindowHandle, WM_KEYUP, (int)VirtualKeyCodes.VK_SHIFT, 0xC02A0001);
        }



        public void SendKey(Process process, VirtualKeyCodes key, uint repeatCount = 1, uint previousState = 0)
        {
            //http://www.codingvision.net/miscellaneous/c-send-text-to-notepad
            //http://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters

            var processWindowHandle = FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, null);
            var lparam = CreateParam(repeatCount, (uint)key, 0, 0, previousState);
            PostMessage(processWindowHandle, WM_KEYDOWN, (int)key, lparam);
        }

        private uint CreateMouseParam(Point point)
        {
            return (uint)((point.Y << 16) | (point.X & 0xFFFF));
        }

        private uint CreateParam(uint repeatCount, uint scanCode, uint extended = 0, uint context = 0, uint previousState = 0, uint transition = 0)
        {
            return repeatCount
                | (scanCode << 16)
                | (extended << 24)
                | (context << 29)
                | (previousState << 30)
                | (transition << 31);
        }
    }
}
