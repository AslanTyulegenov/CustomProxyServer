namespace CustomProxyServer.Controllers;

public class MainController : IController
{
    public string Index() => "Hello world!!!";

    public User[] Users()
    {
        Thread.Sleep(50);
       
        return new[]
        {
            new User("Aslan", 22),
            new User("Asiya", 18),
            new User("Stella", 48),
        };
    } 
    
    public async Task<User[]> UsersAsync()
    {
        Thread.Sleep(50);
       
        return new[]
        {
            new User("Aslan", 22),
            new User("Asiya", 18),
            new User("Stella", 48),
        };
    } 
}

public record User(string Name, int Age);
