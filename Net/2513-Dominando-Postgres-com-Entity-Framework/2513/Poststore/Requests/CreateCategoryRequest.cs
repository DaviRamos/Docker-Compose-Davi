namespace Poststore.Requests;

public class CreateCategoryRequest
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}