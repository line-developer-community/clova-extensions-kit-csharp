using Microsoft.Extensions.Logging;

namespace LineDC.CEK
{
    public interface ILoggableClova : IClova
    {
        ILogger Logger { get; set; }
    }
}
