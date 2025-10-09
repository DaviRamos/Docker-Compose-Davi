namespace CoffeeShop.Models;

public class ChatMessageModel
{
    public Guid Id { get; set; }
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;

    public Guid ChatId { get; set; }
    public ChatModel ChatModel { get; set; } = null!;
}