using System;
using System.Linq;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleValidatorTests
{
    private readonly CreateSaleValidator _validator = new CreateSaleValidator();

    private static CreateSaleCommand ValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = "S001",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Date = DateTime.UtcNow,
            Items =
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto",
                    Quantity = 4,
                    UnitPrice = 10m
                }
            }
        };
    }

    [Fact(DisplayName = "Comando válido deve passar na validação")]
    public void Valid_Should_Pass()
    {
        var cmd = ValidCommand();
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Sem itens deve falhar")]
    public void No_Items_Should_Fail()
    {
        var cmd = ValidCommand();
        cmd.Items.Clear();

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact(DisplayName = "Item com quantidade 0 deve falhar")]
    public void Item_Qty_Zero_Should_Fail()
    {
        var cmd = ValidCommand();
        cmd.Items.First().Quantity = 0;

        var result = _validator.TestValidate(cmd);

        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }
}
