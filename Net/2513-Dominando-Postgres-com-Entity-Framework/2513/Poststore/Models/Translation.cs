namespace Poststore.Models;

public class Translation
{
    public string Language { get; set; } = string.Empty;
    public Heading Heading { get; set; } = new();
}