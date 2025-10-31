using System;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public class SaleTests
{
    [Fact(DisplayName = "Adicionar item válido deve atualizar coleção e TotalAmount")]
    public void AddItem_Should_Add_And_Update_Total()
    {
        // Arrange
        var sale = new Sale { SaleNumber = "S001" };

        // Act
        sale.AddItem(productId: Guid.NewGuid().ToString(), productName: "P1", quantity: 4, unitPrice: 10m);

        // Assert
        sale.Items.Should().HaveCount(1);
        // qty=4 => desconto automático 10% => preço efetivo = 9 => total=36
        sale.TotalAmount.Should().Be(36m);
    }

    [Fact(DisplayName = "Cancelar venda deve marcar Cancelled e atualizar UpdatedAt")]
    public void Cancel_Should_Set_Cancelled()
    {
        var sale = new Sale();
        sale.Cancel();

        sale.Cancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve permitir adicionar item quando venda estiver cancelada")]
    public void AddItem_On_Cancelled_Sale_Should_Throw()
    {
        var sale = new Sale();
        sale.Cancel();

        var act = () => sale.AddItem(Guid.NewGuid().ToString(), "Produto", 4, 10m);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*cancelada*");
    }

    [Fact(DisplayName = "Quantidade > 20 não é permitida")]
    public void AddItem_Qty_Above_20_Should_Throw()
    {
        var sale = new Sale();
        var act = () => sale.AddItem(Guid.NewGuid().ToString(), "Produto", 21, 10m);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*20 unidades*");
    }

    [Fact(DisplayName = "Desconto informado com qty < 4 deve falhar")]
    public void AddItem_Explicit_Discount_With_Qty_Less_Than_4_Should_Throw()
    {
        var sale = new Sale();
        var act = () => sale.AddItem(Guid.NewGuid().ToString(), "Produto", 2, 10m, discountPercent: 0.05m);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*menos de 4 itens*");
    }

    [Theory(DisplayName = "Desconto fora do intervalo [0..1] deve falhar")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void AddItem_Discount_Out_Of_Range_Should_Throw(double bad)
    {
        var sale = new Sale();
        var act = () => sale.AddItem(Guid.NewGuid().ToString(), "Produto", 4, 10m, discountPercent: (decimal)bad);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
