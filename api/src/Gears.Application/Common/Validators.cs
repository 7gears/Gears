namespace Gears.Application.Common;

public static class Validators
{
    public static IRuleBuilderOptions<T, string> IsNotEmpty<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty()
            .WithMessage("{PropertyName} is required");
}
