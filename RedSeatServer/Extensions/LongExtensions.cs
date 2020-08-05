using System;

namespace RedSeatServer.Extensions
{
    public static class LongExtensions
    {
    static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
     const long byteConversion = 1000;
    public static string HumanReadableFileSize(this long value)
    {

        if (value < 0) { return "-" + HumanReadableFileSize(-value); }
        if (value == 0) { return "0.0 bytes"; }

        int mag = (int)Math.Log(value, byteConversion);
        double adjustedSize = (value / Math.Pow(1000, mag));


        return string.Format("{0:n2} {1}", adjustedSize, SizeSuffixes[mag]);
    }
    }
}