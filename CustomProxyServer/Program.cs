
using System.Reflection;
using CustomProxyServer;

var projectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

var host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
await host.StartAsync();