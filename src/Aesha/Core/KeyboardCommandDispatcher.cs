using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Aesha.Infrastructure;
using Aesha.Interfaces;

namespace Aesha.Core
{
    public class KeyboardCommandDispatcher
    {
        private readonly IntPtr _processWindowHandle;

        private readonly Dictionary<char, KeyMap> _keyMaps = new Dictionary<char, KeyMap>()
        {

            {'!', new KeyMap() {ScanCode = 0x2, VirtualKeyCode = '1', Shifted = true}},
            {'"', new KeyMap() {ScanCode = 0x3, VirtualKeyCode = '2', Shifted = true}},
            {'£', new KeyMap() {ScanCode = 0x4, VirtualKeyCode = '3', Shifted = true}},
            {'$', new KeyMap() {ScanCode = 0x5, VirtualKeyCode = '4', Shifted = true}},
            {'%', new KeyMap() {ScanCode = 0x6, VirtualKeyCode = '5', Shifted = true}},
            {'^', new KeyMap() {ScanCode = 0x7, VirtualKeyCode = '6', Shifted = true}},
            {'&', new KeyMap() {ScanCode = 0x8, VirtualKeyCode = '7', Shifted = true}},
            {'*', new KeyMap() {ScanCode = 0x9, VirtualKeyCode = '8', Shifted = true}},
            {'(', new KeyMap() {ScanCode = 0x0A, VirtualKeyCode = '9', Shifted = true}},
            {')', new KeyMap() {ScanCode = 0x0B, VirtualKeyCode = '0', Shifted = true}},
            {'_', new KeyMap() {ScanCode = 0x0C, VirtualKeyCode = '-', Shifted = true}},
            {'+', new KeyMap() {ScanCode = 0x0D, VirtualKeyCode = '=', Shifted = true}},
            {'Q', new KeyMap() {ScanCode = 0x10, VirtualKeyCode = 'Q'}},
            {'W', new KeyMap() {ScanCode = 0x11, VirtualKeyCode = 'W'}},
            {'E', new KeyMap() {ScanCode = 0x12, VirtualKeyCode = 'E'}},
            {'R', new KeyMap() {ScanCode = 0x13, VirtualKeyCode = 'R'}},
            {'T', new KeyMap() {ScanCode = 0x14, VirtualKeyCode = 'T'}},
            {'Y', new KeyMap() {ScanCode = 0x15, VirtualKeyCode = 'Y'}},
            {'U', new KeyMap() {ScanCode = 0x16, VirtualKeyCode = 'U'}},
            {'I', new KeyMap() {ScanCode = 0x17, VirtualKeyCode = 'I'}},
            {'O', new KeyMap() {ScanCode = 0x18, VirtualKeyCode = 'O'}},
            {'P', new KeyMap() {ScanCode = 0x19, VirtualKeyCode = 'P'}},
            {'{', new KeyMap() {ScanCode = 0x1A, VirtualKeyCode = '{'}},
            {'}', new KeyMap() {ScanCode = 0x1B, VirtualKeyCode = '}'}},
            {'A', new KeyMap() {ScanCode = 0x1E, VirtualKeyCode = 'A'}},
            {'S', new KeyMap() {ScanCode = 0x1F, VirtualKeyCode = 'S'}},
            {'D', new KeyMap() {ScanCode = 0x20, VirtualKeyCode = 'D'}},
            {'F', new KeyMap() {ScanCode = 0x21, VirtualKeyCode = 'F'}},
            {'G', new KeyMap() {ScanCode = 0x22, VirtualKeyCode = 'G'}},
            {'H', new KeyMap() {ScanCode = 0x23, VirtualKeyCode = 'H'}},
            {'J', new KeyMap() {ScanCode = 0x24, VirtualKeyCode = 'J'}},
            {'K', new KeyMap() {ScanCode = 0x25, VirtualKeyCode = 'K'}},
            {'L', new KeyMap() {ScanCode = 0x26, VirtualKeyCode = 'L'}},
            {':', new KeyMap() {ScanCode = 0x27, VirtualKeyCode = ';', Shifted = true}},
            {'@', new KeyMap() {ScanCode = 0x28, VirtualKeyCode = '\'', Shifted = true}},
            {'|', new KeyMap() {ScanCode = 0x2B, VirtualKeyCode = '\\', Shifted = true}},
            {'~', new KeyMap() {ScanCode = 0x2B, VirtualKeyCode = '#', Shifted = true}},
            {'Z', new KeyMap() {ScanCode = 0x2C, VirtualKeyCode = 'Z'}},
            {'X', new KeyMap() {ScanCode = 0x2D, VirtualKeyCode = 'X'}},
            {'C', new KeyMap() {ScanCode = 0x2E, VirtualKeyCode = 'C'}},
            {'V', new KeyMap() {ScanCode = 0x2F, VirtualKeyCode = 'V'}},
            {'B', new KeyMap() {ScanCode = 0x30, VirtualKeyCode = 'B'}},
            {'N', new KeyMap() {ScanCode = 0x31, VirtualKeyCode = 'N'}},
            {'M', new KeyMap() {ScanCode = 0x32, VirtualKeyCode = 'M'}},
            {'<', new KeyMap() {ScanCode = 0x33, VirtualKeyCode = ',', Shifted = true}},
            {'>', new KeyMap() {ScanCode = 0x34, VirtualKeyCode = '.', Shifted = true}},
            {'?', new KeyMap() {ScanCode = 0x35, VirtualKeyCode = '/', Shifted = true}},
            {'1', new KeyMap() {ScanCode = 0x2, VirtualKeyCode = '1'}},
            {'2', new KeyMap() {ScanCode = 0x3, VirtualKeyCode = '2'}},
            {'3', new KeyMap() {ScanCode = 0x4, VirtualKeyCode = '3'}},
            {'4', new KeyMap() {ScanCode = 0x5, VirtualKeyCode = '4'}},
            {'5', new KeyMap() {ScanCode = 0x6, VirtualKeyCode = '5'}},
            {'6', new KeyMap() {ScanCode = 0x7, VirtualKeyCode = '6'}},
            {'7', new KeyMap() {ScanCode = 0x8, VirtualKeyCode = '7'}},
            {'8', new KeyMap() {ScanCode = 0x9, VirtualKeyCode = '8'}},
            {'9', new KeyMap() {ScanCode = 0x0A, VirtualKeyCode = '9'}},
            {'0', new KeyMap() {ScanCode = 0x0B, VirtualKeyCode = '0'}},
            {'-', new KeyMap() {ScanCode = 0x0C, VirtualKeyCode = '-'}},
            {'=', new KeyMap() {ScanCode = 0x0D, VirtualKeyCode = '='}},
            {'[', new KeyMap() {ScanCode = 0x1A, VirtualKeyCode = '['}},
            {']', new KeyMap() {ScanCode = 0x1B, VirtualKeyCode = ']'}},
            {';', new KeyMap() {ScanCode = 0x27, VirtualKeyCode = ';'}},
            {'\'', new KeyMap() {ScanCode = 0x28, VirtualKeyCode = '\''}},
            {'`', new KeyMap() {ScanCode = 0x29, VirtualKeyCode = '`'}},
            {'\\', new KeyMap() {ScanCode = 0x2B, VirtualKeyCode = '\\'}},
            {'#', new KeyMap() {ScanCode = 0x2B, VirtualKeyCode = '#'}},
            {',', new KeyMap() {ScanCode = 0x33, VirtualKeyCode = ','}},
            {'.', new KeyMap() {ScanCode = 0x34, VirtualKeyCode = '.'}},
            {'/', new KeyMap() {ScanCode = 0x35, VirtualKeyCode = '/'}},
            {' ', new KeyMap() {ScanCode = 0x39, VirtualKeyCode = ' '}}
        };
        
        public KeyboardCommandDispatcher(IWowProcess process)
        {
            _processWindowHandle = process.MainWindowHandle;
        }

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;


        public void SendShiftKey(char key)
        {
            var map = MapKey(key);
            if (map == null) throw new Exception($"Unable to map key: {key}");

            InternalSetKeyDown((int)Keys.ShiftKey);
            InternalSendKeyDown(0x2A, (int)Keys.ShiftKey);
            InternalSendKeyDown(map.ScanCode, map.VirtualKeyCode);
            InternalSendKeyUp(map.ScanCode, map.VirtualKeyCode);
            InternalSendKeyUp(0x2A,(int)Keys.ShiftKey);
            InternalSetKeyUp((int)Keys.ShiftKey);
        }

        public void SendKeys(string keys)
        {
            foreach (var key in keys)
            {
                SendKey(key);
            }
        }

        public void SendKey(char key)
        {
            var map = MapKey(key);
            if (map != null && map.Shifted) {
                SendShiftKey(key);
                return;
            }
            if (map == null) throw new Exception($"Unable to map key: {key}");

            InternalSendKeyDown(map.ScanCode, map.VirtualKeyCode);
            InternalSendKeyUp(map.ScanCode, map.VirtualKeyCode);
        }

        private KeyMap MapKey(char key)
        {
            var upper = char.IsUpper(key);
            key = key.ToString().ToUpper().ToCharArray().First();
            var map = _keyMaps.ContainsKey(key) ? _keyMaps[key] : null;
            if (upper)
                map.Shifted = true;

            return map;
        }

        private void InternalSetKeyDown(int virtualKeyCode)
        {
            var foreignThreadId = Win32Imports.GetWindowThreadProcessId(_processWindowHandle, IntPtr.Zero);
            var localThreadId = Win32Imports.GetCurrentThreadId();
            Win32Imports.AttachThreadInput(localThreadId, foreignThreadId, true);

            var keys = new byte[256];
            Win32Imports.GetKeyboardState(keys);
            keys[virtualKeyCode] |= 0x80;
            Win32Imports.SetKeyboardState(keys);

            Thread.Sleep(50);
        }

        private void InternalSetKeyUp(int virtualKeyCode)
        {
            var keys = new byte[256];
            Win32Imports.GetKeyboardState(keys);
            keys[virtualKeyCode] &= 0x00;
            Win32Imports.SetKeyboardState(keys);

            var foreignThreadId = Win32Imports.GetWindowThreadProcessId(_processWindowHandle, IntPtr.Zero);
            var localThreadId = Win32Imports.GetCurrentThreadId();
            Win32Imports.AttachThreadInput(localThreadId, foreignThreadId, false);

            Thread.Sleep(50);
        }

        private void InternalSendKeyDown(int scanCode, int virtualKeyCode)
        {
            var downParam = CreateParam(1, (uint)scanCode, 0, 0, 0, 0);
            Win32Imports.PostMessage(_processWindowHandle, WM_KEYDOWN, virtualKeyCode, downParam);

            Thread.Sleep(50);
        }


        private void InternalSendKeyUp(int scanCode, int virtualKeyCode)
        {
            var upParam = CreateParam(1, (uint)scanCode, 0, 0, 1, 1);
            Win32Imports.PostMessage(_processWindowHandle, WM_KEYUP, virtualKeyCode, upParam);

            Thread.Sleep(50);
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

        public void SendKeyDown(char key)
        {
            var map = MapKey(key);
            InternalSendKeyDown(map.ScanCode, map.VirtualKeyCode);
        }

        public void SendKeyUp(char key)
        {
            var map = MapKey(key);
            InternalSendKeyUp(map.ScanCode, map.VirtualKeyCode);
        }

        public void SetPlayerFacing(float newFacing)
        {
            var currentFacing = ObjectManager.Me.Rotation;
            var diff = newFacing - currentFacing;
            var direction = diff < 0 ? 'D' : 'A';
            
            while (diff < -0.1 || diff > 0.1)
            {
                SendKeyDown(direction);
                diff = newFacing - ObjectManager.Me.Rotation;
            }

            SendKeyUp(direction);
        }

    }

    class KeyMap
    {
        public int ScanCode { get; set; }
        public int VirtualKeyCode { get; set; }
        public bool Shifted { get; set; }
    }
}
