using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;

    public bool Cancelled { get; private set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public decimal TotalAmount => Math.Round(Items.Sum(i => i.TotalAmount), 2);

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(string productId, string productName, int quantity, decimal unitPrice, decimal? discountPercent = null)
    {
        if (Cancelled)
            throw new InvalidOperationException("Não é possível adicionar itens em uma venda cancelada.");

        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");
        if (quantity > 20)
            throw new InvalidOperationException("Não é permitido vender mais de 20 unidades do mesmo produto.");
        if (unitPrice < 0)
            throw new ArgumentException("O preço unitário deve ser maior ou igual a zero.");

        decimal descontoAutomatico =
            quantity >= 10 ? 0.20m :
            quantity >= 4 ? 0.10m : 0.00m;

        if (quantity < 4 && (discountPercent ?? 0) > 0)
            throw new InvalidOperationException("Compras com menos de 4 itens não podem ter desconto.");

        var descontoFinal = discountPercent ?? descontoAutomatico;
        if (descontoFinal < 0 || descontoFinal > 1)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "O desconto deve estar entre 0 e 1 (0% a 100%).");

        var item = new SaleItem
        {
            SaleId = Id,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            DiscountPercent = descontoFinal
        };

        item.Validate();
        Items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Cancelled)
            throw new InvalidOperationException("A venda já está cancelada.");

        Cancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
