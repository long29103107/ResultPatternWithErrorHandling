namespace ResultPatternExample.Requests;

public class CreateTodoRequest
{
    public int UserId { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
}
