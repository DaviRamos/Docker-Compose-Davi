namespace CoffeeShop.Models;

public class ChatModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ChatMessageModel> Messages { get; set; } = [];
}