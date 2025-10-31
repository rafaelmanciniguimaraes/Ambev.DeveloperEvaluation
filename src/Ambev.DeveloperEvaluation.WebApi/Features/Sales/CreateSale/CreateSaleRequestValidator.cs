using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A venda deve conter pelo menos um item.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("O produto é obrigatório.");

            items.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.");

            items.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
                .LessThanOrEqualTo(20).WithMessage("Não é permitido vender acima de 20 itens idênticos.");

            items.RuleFor(i => i.UnitPrice)
                .GreaterThan(0m).WithMessage("O preço unitário deve ser maior que zero.");

            items.RuleFor(i => i.DiscountPercent)
            .Must(dp => !dp.HasValue || (dp.Value >= 0m && dp.Value <= 1m) || (dp.Value >= 0m && dp.Value <= 100m))
            .WithMessage("O desconto deve estar entre 0%–100%");

            // Proíbe desconto quando quantidade < 4
            items.RuleFor(i => i)
                .Must(i => i.Quantity >= 4)
                .WithMessage("Compras com menos de 4 itens não podem ter desconto.");
        });
    }
}