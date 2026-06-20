using FluentValidation;

namespace Api.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var model = ctx.Arguments.OfType<T>().FirstOrDefault();

        if (model is null)
            return Results.BadRequest(new { error = "Corpo da requisição inválido." });

        var result = await validator.ValidateAsync(model, ctx.HttpContext.RequestAborted);

        if (!result.IsValid)
            return Results.BadRequest(new { errors = result.Errors.Select(e => e.ErrorMessage) });

        return await next(ctx);
    }
}
