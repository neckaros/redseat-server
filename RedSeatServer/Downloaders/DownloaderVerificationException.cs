using System;

namespace RedSeatServer.Downloaders
{
public class DownloaderVerificationException : Exception
{
    public DownloaderVerificationException()
    {
    }

    public DownloaderVerificationException(string message)
        : base(message)
    {
    }

    public DownloaderVerificationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
}