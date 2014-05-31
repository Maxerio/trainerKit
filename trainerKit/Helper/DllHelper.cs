using System;
using System.Diagnostics;
namespace trainerKit.Helper
{
    internal static class DllHelper
    {
        public static IntPtr GetDllBaseAddress(string dllName, string processName, int processIndex = 0)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            Process process = processes[processIndex];

            ProcessModuleCollection modules = process.Modules;
            ProcessModule dllBaseAdress = null;
            foreach (ProcessModule i in modules)
            {
                if (i.ModuleName == dllName)
                {
                    dllBaseAdress = i;
                    break;
                }
            }

            return dllBaseAdress.BaseAddress;
        }
    }
}

