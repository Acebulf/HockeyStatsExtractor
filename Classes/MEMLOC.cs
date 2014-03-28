using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsExtractorConsole2
{
    static class MEMLOC
    {
        //Location in memory of variables.
        const int MEMLOC_posX = 0x07D1C290;
        const int MEMLOC_posY = 0x07D1C298;
        const int MEMLOC_posZ = 0x07D1C294;

        const int MEMLOC_vX = 0x07D1C2F4;
        const int MEMLOC_vY = 0x07D1C2D8;

        public const int Time = 0x07D349A8; //Type = Int32
        public const int Period = 0x07D349B0; //Type = Int32
        public const int StopTime = 0x07D33DA0; //Type = Int32

        public const int RedScore = 0x07D33D98;
        public const int BlueScore = 0x07D33D9C;

        public const int PlayerList = 0x04A5B860;
    }
}
