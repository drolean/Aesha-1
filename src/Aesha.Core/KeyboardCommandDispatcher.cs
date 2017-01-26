using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aesha.Objects.Infrastructure;

namespace Aesha.Core
{
    public class KeyboardCommandDispatcher
    {
        //http://www.codingvision.net/miscellaneous/c-send-text-to-notepad
        //http://stackoverflow.com/questions/21994276/setting-wm-keydown-lparam-parameters

        private readonly IntPtr _processWindowHandle;

        private readonly Dictionary<char, KeyMap> _keyMaps = new Dictionary<char, KeyMap>()
        {
            {'A', new KeyMap() {ScanCode = 0x1E, VirtualKeyCode = 'A'}}
        };
        
        public KeyboardCommandDispatcher(Process process)
        {
            _processWindowHandle = Win32Imports.FindWindowEx(process.MainWindowHandle, IntPtr.Zero, null, null);
        }

        #region VirtualKeyCodes Enum

        private enum VirtualKeyCodes
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


        public void SendShiftKey(string key)
        {
            InternalSendKeyDown(0x2A, (int) VirtualKeyCodes.VK_SHIFT);
            Thread.Sleep(500);
            SendKey(key);
            Thread.Sleep(500);
            InternalSendKeyUp(0x2A,(int)VirtualKeyCodes.VK_SHIFT);
        }

        public void SendKey(string key)
        {
            var map = MapKey(key);
            InternalSendKeyDown(map.ScanCode, map.VirtualKeyCode);
            InternalSendKeyUp(map.ScanCode, map.VirtualKeyCode);
        }

        private KeyMap MapKey(string key)
        {
            var keyChar = key.ToUpper().ToCharArray().First();
            return _keyMaps[keyChar];
        }

        private void InternalSendKeyDown(int scanCode, int virtualKeyCode)
        {
            var downParam = CreateParam(1, (uint)scanCode, 0, 0, 0, 0);
            Win32Imports.PostMessage(_processWindowHandle, WM_KEYDOWN, virtualKeyCode, downParam);
        }


        private void InternalSendKeyUp(int scanCode, int virtualKeyCode)
        {
            var upParam = CreateParam(1, (uint)scanCode, 0, 0, 1, 1);
            Win32Imports.PostMessage(_processWindowHandle, WM_KEYUP, virtualKeyCode, upParam);
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

    class KeyMap
    {
        public int ScanCode { get; set; }
        public int VirtualKeyCode { get; set; }
    }
}
