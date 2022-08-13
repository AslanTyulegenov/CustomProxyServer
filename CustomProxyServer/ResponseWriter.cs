using System.Net;

namespace CustomProxyServer;

public static class ResponseWriter
{
    public static void WriteStatus(HttpStatusCode code, Stream stream)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        writer.WriteLine($"HTTP/1.0 {(int)code} {code.ToString()}");
        writer.WriteLine();
    }
    
    public static async Task WriteStatusAsync(HttpStatusCode code, Stream stream)
    {
        using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteLineAsync($"HTTP/1.0 {(int)code} {code.ToString()}");
        await writer.WriteLineAsync();
    }
}
