using Application.Orders.DTOs;
using Domain.Enums;
using FluentValidation;

namespace Api.Validators.Orders;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("O tipo do pedido é obrigatório.")
            .Must(t => Enum.TryParse<OrderType>(t, ignoreCase: true, out _))
            .WithMessage("Tipo inválido. Valores aceitos: standard, express, subscription.");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("A lista de itens é obrigatória.")
            .NotEmpty().WithMessage("O pedido deve conter ao menos um item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("O ID do produto é obrigatório.");
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        });
    }
}
