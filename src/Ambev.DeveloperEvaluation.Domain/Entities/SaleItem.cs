using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }             
    public Sale? Sale { get; set; }               

    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    
    public decimal? DiscountPercent { get; set; }

    
    public decimal LineTotal => Math.Round(UnitPrice * Quantity, 2);
    public decimal DiscountAmount => Math.Round(LineTotal * DiscountPercent!.Value, 2);

    
    public decimal TotalAmount => Math.Round(LineTotal - DiscountAmount, 2);

    public ValidationResultDetail Validate()
    {
        var result = new ValidationResultDetail { IsValid = true, Errors = Enumerable.Empty<ValidationErrorDetail>() };

        if (Quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");
        if (Quantity > 20)
            throw new InvalidOperationException("Não é permitido vender mais de 20 unidades do mesmo produto.");
        if (UnitPrice < 0)
            throw new ArgumentException("O preço unitário deve ser maior ou igual a zero.");
        if (DiscountPercent < 0 || DiscountPercent > 1)
            throw new ArgumentOutOfRangeException(nameof(DiscountPercent), "O desconto deve estar entre 0 e 1 (0% a 100%).");

        return result;
    }
}
