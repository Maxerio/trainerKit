using System;
using System.Collections.Generic;
using System.Text;
using trainerKit.Handler;
using trainerKit.Helper;

namespace trainerKit
{
    public class PointerModel
    {

        private UInt32? dmaAddress;

        private UInt32 internalBaseAddress;

        public UInt32[] Offsets
        {
            get;
            set;
        }
        /// <summary>
        /// BaseAddress is required
        /// </summary>
        /// <value>The base address.</value>
        public UInt32 BaseAddress
        {
            get;
            set;
        }

        public byte[] Value
        {
            get;
            set;
        }

        public string DllName 
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The dma address.</returns>
        /// <param name="processName">processName.</param>
        /// <param name="processHandle">Process handle.</param>
        public UInt32 GetDmaAddress(string processName, IntPtr processHandle)
        {
            if (this.dmaAddress == null) {
                if (!string.IsNullOrWhiteSpace (this.DllName)) 
                {
                    this.internalBaseAddress = (UInt32)DllHelper.GetDllBaseAddress (this.DllName, processName).ToInt32();
                    this.internalBaseAddress += this.BaseAddress;
                }
                else
                {
                    this.internalBaseAddress = this.BaseAddress;
                }

                if (this.Offsets != null && this.Offsets.Length > 0) {
                    this.dmaAddress = FindDmaAddress (this.Offsets.Length, processHandle, this.Offsets, this.internalBaseAddress);
                } 
                else 
                {
                    this.dmaAddress = internalBaseAddress;
                }
            } 
            return this.dmaAddress.Value;
        }

        public static byte[] ValueToByte(int val)
        {
            byte[] output = new byte[4];
            output[3] = Convert.ToByte((val >> 24) & 0xFF);
            output[2] = Convert.ToByte((val >> 16) & 0xFF);
            output[1] = Convert.ToByte((val >> 8) & 0xFF);
            output[0] = Convert.ToByte(val & 0xFF);
            return output;
        }

        public static UInt32 FindDmaAddress(int pointerLevel, IntPtr hProcHandle, UInt32[] offsets, UInt32 baseAddress)
        {
            UInt32 pointer = baseAddress;

            UInt32 pTemp = 0;
            byte[] buffer = new byte[24];
            UInt32 pointerAddr = 0;
            IntPtr tmp;
            for(int i = 0; i < pointerLevel; i ++)
            {
                if(i == 0)
                {
                    WinApiHandler.ReadProcessMemory(hProcHandle, (IntPtr)pointer, buffer, 4, out tmp);
                }
                pTemp = BitConverter.ToUInt32(buffer, 0);
                pointerAddr = pTemp + offsets[i];
                WinApiHandler.ReadProcessMemory(hProcHandle, (IntPtr)pointerAddr, buffer, 4, out tmp);
            }
            return pointerAddr;
        }
    }
}
