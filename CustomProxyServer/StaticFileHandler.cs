using System.Net;

namespace CustomProxyServer;

public class StaticFileHandler : IHandler
{
    private readonly string _path;

    public StaticFileHandler(string path)
    {
        _path = path;
    }

    public void Handle(Stream networkStream, Request request)
    {
        var filePath = Path.Combine(_path, request.Path.Substring(1));

        if (!File.Exists(filePath))
        {
            ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);

            return;
        }

        ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);

        using(var fileStream = File.OpenRead(filePath))
        {
            fileStream.CopyTo(networkStream);
        }
    }
    public async Task HandleAsync(Stream networkStream, Request request)
    {
        var filePath = Path.Combine(_path, request.Path.Substring(1));

        if (!File.Exists(filePath))
        {
            await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, networkStream);

            return;
        }

        await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, networkStream);

        await using(var fileStream = File.OpenRead(filePath))
        {
            await fileStream.CopyToAsync(networkStream);
        }
    }
}
