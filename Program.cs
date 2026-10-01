using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var books = new List<Book>
{
    new Book(1, "The Beatles Anthology", "music"),[cite: 3]
    new Book(2, "Miles Davis Autobiography", "music"),[cite: 3]
    new Book(3, "Clean Code", "programming"),
    new Book(4, "Refactoring", "programming")
};

app.UseMiddleware<AuthMiddleware>();

app.MapGet("/allbooks", () => RenderBooksTable(books)); [cite: 3]

app.MapGet("/getbooks", (string? category) => [cite: 3]
{
    var filtered = string.IsNullOrEmpty(category)
        ? books
        : books.Where(b => b.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

    return RenderBooksTable(filtered);
});

app.Run();

IResult RenderBooksTable(IEnumerable<Book> bookList)
{
    var sb = new StringBuilder();
    sb.Append("<table border='1' style='border-collapse: collapse; padding: 8px;'>");
    sb.Append("<tr><th>ID</th><th>Title</th><th>Category</th></tr>");

    foreach (var b in bookList)
    {
        sb.Append($"<tr><td>{b.Id}</td><td>{b.Title}</td><td>{b.Category}</td></tr>");
    }

    sb.Append("</table>");
    return Results.Content(sb.ToString(), "text/html", Encoding.UTF8); [cite: 3]
}

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/getbooks")) [cite: 3]
        {
            var token = context.Request.Query["token"].ToString(); [cite: 3]
            
            if (token != "token12345") [cite: 3]
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid or missing token.");
                return;
            }
        }

        await _next(context);
    }
}

public record Book(int Id, string Title, string Category);
