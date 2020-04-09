using System;

namespace CrazySharp.Base.Enums
{
    [Flags]
    public enum FileTypeFilter
    {
        ALL = 0x00000001,
        BIN = 0x00000002,
        PBIN = 0x00000004,
        TXT = 0x00000008,
        JSON =0x00000010,
        EXCEL = 0x00000020,
        WORD = 0x00000040,
        PPT = 0x00000080,
        LICENSE=0x00000100,
        IMAGE = 0x00000200,
        PSDATA =0x00000400,
        PPBAK=0x00000800,
    }
}