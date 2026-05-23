namespace Shared.Component.Extensions;

public static class NameExtensions
{
    public static string GetFirstName(this string name)
    {
        var split = name.Split(" ");

        return split[0];
    }
}