using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;

    public GetSaleHandler(ISaleRepository repo, IMapper mapper)
    {
        _repo = repo; _mapper = mapper;
    }

    public async Task<GetSaleResult> Handle(GetSaleCommand request, CancellationToken ct)
    {
        var v = new GetSaleValidator();
        var r = await v.ValidateAsync(request, ct);
        if (!r.IsValid) throw new ValidationException(r.Errors);

        var sale = await _repo.GetByIdAsync(request.Id, ct) ?? throw new KeyNotFoundException($"Sale {request.Id} not found");
        var teste =  _mapper.Map<GetSaleResult>(sale);
        return teste;
    }
}
