using System;
using trainerKit.Handler;

namespace trainerKit
{
    public class Window
    {
        private IntPtr Hwnd = IntPtr.Zero;
        private IntPtr HProcHandle = IntPtr.Zero;

        /// <summary>
        /// The name of the window.
        /// </summary>
        /// <value>The name of the window.</value>
        public string WindowName 
        {
            get;
            private set;
        }

        public string ProcessName 
        {
            get;
            private set;
        }

        /// <summary>
        /// '
        /// </summary>
        /// <param name="name">Name.</param>
        public Window (string windowName, string processName)
        {
            this.WindowName = windowName;
            this.ProcessName = processName;
        }

        public bool IsWindowAvailable
        {
            get 
            {
                return this.Hwnd != IntPtr.Zero && this.HProcHandle != IntPtr.Zero;
            }
        }

        /// <summary>
        /// while(Init())
        /// {
        ///     Thread.Sleep(1000);
        /// }
        /// </summary>
        public bool Init() 
        {
            this.Hwnd = WinApiHandler.FindWindow(null, WindowName);
            uint dwProcId = 0;
            WinApiHandler.GetWindowThreadProcessId(this.Hwnd, out dwProcId);
            this.HProcHandle = WinApiHandler.OpenProcess((int)ProcessAccessFlags.All, false, Convert.ToInt32(dwProcId));

            return this.IsWindowAvailable;
        }

        /// <summary>
        /// Writes to memory.
        /// </summary>
        /// <param name="hProcHandle">H proc handle.</param>
        /// <param name="addressToWrite">Address to write.</param>
        /// <param name="value">Value.</param>
        private void WriteToMemory(IntPtr hProcHandle, UInt32 addressToWrite, byte[] value)
        {
            UIntPtr tmp;
            WinApiHandler.WriteProcessMemory( hProcHandle, (IntPtr)addressToWrite, value, (uint)value.Length, out tmp);
        }

        public byte[] ReadMemory(PointerModel data)
        {
            byte[] buffer = new byte[24];
            IntPtr tmp;
            WinApiHandler.ReadProcessMemory(this.HProcHandle, (IntPtr)data.GetDmaAddress(this.ProcessName, this.HProcHandle), buffer, 4, out tmp);
            return buffer;
        }

        /// <summary>
        /// Inject the specified data.
        /// </summary>
        /// <param name="data">Data.</param>
        public void Inject(PointerModel data) 
        {
            if (this.HProcHandle != IntPtr.Zero)
            {
                WriteToMemory(this.HProcHandle, data.GetDmaAddress(this.ProcessName, this.HProcHandle), data.Value);
            }
        } 
    }
}

