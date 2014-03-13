using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace StatsExtractorConsole2
{
    class MemoryExtractor
    {
        IntPtr processManager;

        byte[] refreshData;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public void getRefreshData(int dataLocation, int bufferLength)
            //Stores data during a refresh for less IO calls.
        {
            refreshData = readMemory(dataLocation, bufferLength);
        }

        public byte[] readMemory(int dataLocation, int bufferLength)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[bufferLength];
            ReadProcessMemory((int)processManager, dataLocation, buffer, bufferLength, ref bytesRead);
            return buffer;
        }

        public int getInt32(int dataLocation)
        {
            byte[] buffer = readMemory(dataLocation, 4);
            return System.BitConverter.ToInt32(buffer, 0);
        }

        public int refreshInt32(int offset)
            //Refresh function
        {
            return System.BitConverter.ToInt32(refreshData, offset);
        }

        public bool getBool(int dataLocation)
        {
            byte[] buffer = readMemory(dataLocation, 4);
            return System.BitConverter.ToBoolean(buffer, 0);
        }

        public bool refreshBool(int offset)
        {
            return System.BitConverter.ToBoolean(refreshData, offset);
        }

        public string getString(int dataLocation, int bufferLength)
        {
            byte[] buffer = readMemory(dataLocation, bufferLength);
            string retstr = System.Text.Encoding.UTF8.GetString(buffer, 0, bufferLength).Trim('\0');
            return retstr.Split('\0')[0];
        }

        public string refreshString(int offset, int bufferLength)
        {
            string retstr = System.Text.Encoding.UTF8.GetString(refreshData , offset, bufferLength).Trim('\0');
            return retstr.Split('\0')[0];
        }

        public MemoryExtractor(IntPtr processMng)
        {
            processManager = processMng;
        }

        public MemoryExtractor()
        {
            Process hockeyqm = Process.GetProcessesByName("hockey")[0];
            processManager = OpenProcess(0x0010, false, hockeyqm.Id);
        }

        public MemoryExtractor(string processName)
        {
            Process hockeyqm = Process.GetProcessesByName(processName)[0];
            processManager = OpenProcess(0x0010, false, hockeyqm.Id);
        }
    }
}
