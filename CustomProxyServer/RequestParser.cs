namespace CustomProxyServer;

public static class RequestParser
{
    public static Request Parse(string header)
    {
        var result = header.Split(' ');

        return new Request(result[1], GetMethod(result[0]));
    }

    public static HttpMethod GetMethod(string httpMethod)
    {
        return new HttpMethod(httpMethod);
    }
}