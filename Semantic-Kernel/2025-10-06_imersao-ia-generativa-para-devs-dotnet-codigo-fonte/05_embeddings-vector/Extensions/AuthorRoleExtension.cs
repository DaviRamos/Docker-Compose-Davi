using Microsoft.SemanticKernel.ChatCompletion;

namespace CoffeeShop.Extensions;

public static class AuthorRoleExtension
{
    public static string Map(this AuthorRole role) => role.Label.ToLower();

    public static AuthorRole Map(this string role) =>
        role switch
        {
            "user" => AuthorRole.User,
            "assistant" => AuthorRole.Assistant,
            "developer" => AuthorRole.Developer,
            "system" => AuthorRole.System,
            "tool" => AuthorRole.Tool,
            _ => AuthorRole.User
        };
}