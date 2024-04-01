using OperationsResearch.Enums;

namespace OperationsResearch.Structures;
public struct LogMessageManifest
{
    public LogSeverity Type;
    public string Text;
    public string Time;
    public ulong Id;
}