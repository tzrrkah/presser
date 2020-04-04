using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace presser
{
    internal class DirectInput
    {
        public const int LeftDown = 2;
        public const int LeftUp = 4;
        public const int RightDown = 8;
        public const int RightUp = 16;

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] DirectInput.INPUT[] pInputs, int cbSize);

        public static void SendKey(short keyCode, DirectInput.KeyFlag keyFlag)
        {
            DirectInput.INPUT[] pInputs = new DirectInput.INPUT[1];
            pInputs[0].type = 1;
            pInputs[0].ki.wScan = keyCode;
            pInputs[0].ki.dwFlags = (int)keyFlag;
            pInputs[0].ki.time = 0;
            pInputs[0].ki.dwExtraInfo = IntPtr.Zero;
            int num = (int)DirectInput.SendInput(1U, pInputs, Marshal.SizeOf(typeof(DirectInput.INPUT)));
        }

        public static void PressKey(short key)
        {
            DirectInput.SendKey(key, DirectInput.KeyFlag.Scancode);
            Thread.Sleep(10);
            DirectInput.SendKey(key, DirectInput.KeyFlag.KeyUp | DirectInput.KeyFlag.Scancode);
        }

        public static void PressAndHoldKey(short key)
        {
            DirectInput.SendKey(key, DirectInput.KeyFlag.Scancode);
        }

        public static void ReleaseKey(short key)
        {
            DirectInput.SendKey(key, DirectInput.KeyFlag.KeyUp | DirectInput.KeyFlag.Scancode);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(
          int dwFlags,
          int dx,
          int dy,
          int cButtons,
          int dwExtraInfo);

        public static void LeftMouseClick()
        {
            DirectInput.mouse_event(2, 0, 0, 0, 0);
            Thread.Sleep(10);
            DirectInput.mouse_event(4, 0, 0, 0, 0);
        }

        public static void RightMouseClick()
        {
            DirectInput.mouse_event(8, 0, 0, 0, 0);
            Thread.Sleep(200);
            DirectInput.mouse_event(16, 0, 0, 0, 0);
        }

        public static void LeftMouseClickHold()
        {
            DirectInput.mouse_event(2, 0, 0, 0, 0);
        }

        public static void RightMouseClickHold()
        {
            DirectInput.mouse_event(8, 0, 0, 0, 0);
        }

        public static void LeftMouseClickRelease()
        {
            DirectInput.mouse_event(4, 0, 0, 0, 0);
        }

        public static void RightMouseClickRelease()
        {
            DirectInput.mouse_event(16, 0, 0, 0, 0);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public DirectInput.MOUSEINPUT mi;
            [FieldOffset(4)]
            public DirectInput.KEYBDINPUT ki;
            [FieldOffset(4)]
            public DirectInput.HARDWAREINPUT hi;
        }

        [System.Flags]
        public enum KeyFlag
        {
            KeyDown = 0,
            ExtendKey = 1,
            KeyUp = 2,
            Unicode = 4,
            Scancode = 8,
        }
    }
}

