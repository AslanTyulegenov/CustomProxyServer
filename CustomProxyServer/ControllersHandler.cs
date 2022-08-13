using System.Net;
using System.Reflection;
using System.Text.Json;

namespace CustomProxyServer;

public class ControllersHandler : IHandler
{
    private readonly Dictionary<string, Func<object>> _routers;

    public ControllersHandler(Assembly assembly)
    {
        _routers = assembly.GetTypes()
            .Where(x => typeof(IController).IsAssignableFrom(x))
            .SelectMany(Controller => Controller
                .GetMethods()
                .Select(Method => new
                {
                    Controller, Method
                })).ToDictionary(
            key => GetPath(key.Controller, key.Method),
            value => GetEndpointMethod(value.Controller, value.Method));
    }
    private Func<object> GetEndpointMethod(Type controller, MethodInfo method) => () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
    private string GetPath(Type controller, MethodInfo method)
    {
        var name = controller.Name;

        if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
        {
            name = name.Substring(0, name.Length - "controller".Length);
        }

        if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
        {
            return "/" + name.ToLower();
        }
        else
        {
            return $"/{name}/{method.Name}".ToLower();
        }
    }
    public void Handle(Stream networkStream, Request request)
    {
        if (!_routers.TryGetValue(request.Path, out var func))
            ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
        else
        {
            ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
            WriteControllerResponse(func(), networkStream);
        }
    }

    public async Task HandleAsync(Stream networkStream, Request request)
    {
        if (!_routers.TryGetValue(request.Path, out var func))
            await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, networkStream);
        else
        {
            await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, networkStream);
            await WriteControllerResponseAsync(func(), networkStream);
        }
    }

    private void WriteControllerResponse(object response, Stream networkStream)
    {
        if (response is string str)
        {
            using(var streamWrite = new StreamWriter(networkStream))
            {
                streamWrite.Write(str);
            }
        }
        else if (response is byte[] buffer)
        {
            networkStream.Write(buffer, 0, buffer.Length);
        }
        else
        {
            WriteControllerResponse(JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }),
            networkStream);
        }
    }

    private async Task WriteControllerResponseAsync(object response, Stream networkStream)
    {
        if (response is string str)
        {
            using(var streamWrite = new StreamWriter(networkStream))
            {
                await streamWrite.WriteAsync(str);
            }
        }
        else if (response is byte[] buffer)
        {
            await networkStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else if (response is Task task)
        {
            await task;
            await WriteControllerResponseAsync(task.GetType().GetProperty("Result").GetValue(task), networkStream);
        }
        else
        {
            await WriteControllerResponseAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }),
            networkStream);
        }
    }
}
