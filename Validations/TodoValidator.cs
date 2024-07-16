using FluentValidation;

namespace ResultPatternExample.Validations;

public class TodoValidator : AbstractValidator<Todo>
{
    public TodoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
    }
}
