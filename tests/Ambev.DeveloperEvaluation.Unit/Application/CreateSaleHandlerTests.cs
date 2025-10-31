using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _repo = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<CreateSaleItemCommand, SaleItem>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId.ToString()))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductName))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice))
                .ForMember(d => d.DiscountPercent, o => o.MapFrom(s => s.DiscountPercent));

            c.CreateMap<CreateSaleCommand, Sale>()
                .ForMember(d => d.SaleNumber, o => o.MapFrom(s => s.SaleNumber))
                .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId.ToString()))
                .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.CustomerName))
                .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId.ToString()))
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
        });
        _mapper = cfg.CreateMapper();

        _handler = new CreateSaleHandler(_repo, _mapper, _logger);
    }

    private static CreateSaleCommand BuildCommand(
        int qty = 4, decimal unitPrice = 100m, decimal? discountPercent = null)
    {
        var f = new Faker();
        return new CreateSaleCommand
        {
            SaleNumber = f.Random.AlphaNumeric(8),
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Teste",
            Date = DateTime.UtcNow,
            Items =
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto X",
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    DiscountPercent = discountPercent
                }
            }
        };
    }

    [Fact(DisplayName = "Dado um pedido válido Quando criar venda Então retorna Id da venda")]
    public async Task Handle_Valid_Sale_Should_Return_Id()
    {
        // Arrange
        var cmd = BuildCommand(qty: 5, unitPrice: 50m);
        _repo.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
             .Returns(ci =>
             {
                 var sale = ci.Arg<Sale>();
                 sale.Id = Guid.NewGuid();
                 return sale;
             });

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        await _repo.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Theory(DisplayName = "Aplicar desconto automático de 10% para quantidade entre 4 e 9")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public async Task Handle_Qty_4_to_9_Should_Set_10Percent(int qty)
    {
        // Arrange
        var cmd = BuildCommand(qty: qty, unitPrice: 100m);
        Sale? captured = null;

        _repo.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
             .Returns(ci =>
             {
                 captured = ci.Arg<Sale>();
                 captured!.Id = Guid.NewGuid();
                 return captured;
             });

        // Act
        var _ = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        var item = captured!.Items.Single();
        item.DiscountPercent.Should().Be(0.10m);
    }

    [Theory(DisplayName = "Aplicar desconto automático de 20% para quantidade >= 10")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public async Task Handle_Qty_10_or_More_Should_Set_20Percent(int qty)
    {
        // Arrange
        var cmd = BuildCommand(qty: qty, unitPrice: 100m);
        Sale? captured = null;

        _repo.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                captured = ci.Arg<Sale>();
                captured!.Id = Guid.NewGuid();
                return captured;
            });

        // Act
        var _ = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.Items.Single().DiscountPercent.Should().Be(0.20m);
    }

    [Fact(DisplayName = "Quantidade acima de 20 deve lançar ValidationException")]
    public async Task Handle_Qty_Above_20_Should_Throw()
    {
        // Arrange
        var cmd = BuildCommand(qty: 21, unitPrice: 10m);

        // Act
        var act = () => _handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _repo.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Desconto informado com qty < 4 deve falhar")]
    public async Task Handle_Explicit_Discount_With_Qty_Less_Than_4_Should_Throw()
    {
        var f = new Faker();
        var cmd = new CreateSaleCommand
        {
            SaleNumber = f.Random.AlphaNumeric(8),
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Teste",
            Date = DateTime.UtcNow,
            Items =
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Y",
                    Quantity = 2,
                    UnitPrice = 100m,
                    DiscountPercent = 0.10m
                }
            }
        };

       
        var act = () => _handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _repo.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
