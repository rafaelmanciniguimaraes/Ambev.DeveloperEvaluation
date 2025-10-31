using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository repo, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _repo = repo; _mapper = mapper; _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken ct)
    {
        var val = new CreateSaleValidator();
        var res = await val.ValidateAsync(command, ct);
        if (!res.IsValid) throw new ValidationException(res.Errors);

        var sale = _mapper.Map<Sale>(command);

       
        if (sale.Items == null || !sale.Items.Any())
            throw new ValidationException("A venda deve conter pelo menos um item.");

        foreach (var item in sale.Items.Where(i => i != null))
        {
            if (item.Quantity > 20)
                throw new ValidationException("A 'quantidade' não pode exceder 20.");

            var autoPercent = item.Quantity >= 10 ? 0.20m :
                              item.Quantity >= 4 ? 0.10m : 0.00m;

       
            var explicitPercent = item.DiscountPercent ?? 0m;

            if (item.Quantity < 4 && explicitPercent > 0m)
                throw new ValidationException("Compras com menos de 4 itens não podem ter desconto.");

            item.DiscountPercent = explicitPercent > 0m ? explicitPercent : autoPercent;

            if (item.DiscountPercent < 0m || item.DiscountPercent > 1m)
                throw new ValidationException("O desconto deve estar entre 0% e 100%.");
        }

        var created = await _repo.CreateAsync(sale, ct);
        _logger.LogInformation("SaleCreated {SaleId} {SaleNumber}", created.Id, created.SaleNumber);

        return new CreateSaleResult { Id = created.Id };
    }


}