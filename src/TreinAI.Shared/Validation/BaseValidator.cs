using FluentValidation;

namespace TreinAI.Shared.Validation;

/// <summary>
/// Base validator with common validation rules for tenant-scoped entities.
/// </summary>
public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
{
    /// <summary>
    /// Adds a rule ensuring a string property is not empty and within max length.
    /// </summary>
    protected void RuleForRequiredString(
        System.Linq.Expressions.Expression<Func<T, string>> expression,
        string propertyName,
        int maxLength = 200)
    {
        RuleFor(expression)
            .NotEmpty().WithMessage($"{propertyName} é obrigatório.")
            .MaximumLength(maxLength).WithMessage($"{propertyName} deve ter no máximo {maxLength} caracteres.");
    }

    /// <summary>
    /// Adds a rule ensuring an optional string is within max length when provided.
    /// </summary>
    protected void RuleForOptionalString(
        System.Linq.Expressions.Expression<Func<T, string?>> expression,
        string propertyName,
        int maxLength = 500)
    {
        RuleFor(expression)
            .MaximumLength(maxLength).WithMessage($"{propertyName} deve ter no máximo {maxLength} caracteres.")
            .When(x =>
            {
                var value = expression.Compile()(x);
                return !string.IsNullOrEmpty(value);
            });
    }

    /// <summary>
    /// Adds a rule ensuring a numeric value is positive (> 0).
    /// </summary>
    protected void RuleForPositiveNumber(
        System.Linq.Expressions.Expression<Func<T, decimal>> expression,
        string propertyName)
    {
        RuleFor(expression)
            .GreaterThan(0).WithMessage($"{propertyName} deve ser maior que zero.");
    }

    /// <summary>
    /// Adds a rule for email format validation.
    /// </summary>
    protected void RuleForEmail(
        System.Linq.Expressions.Expression<Func<T, string>> expression,
        string propertyName = "Email")
    {
        RuleFor(expression)
            .NotEmpty().WithMessage($"{propertyName} é obrigatório.")
            .EmailAddress().WithMessage($"{propertyName} deve ser um endereço de email válido.");
    }
}
