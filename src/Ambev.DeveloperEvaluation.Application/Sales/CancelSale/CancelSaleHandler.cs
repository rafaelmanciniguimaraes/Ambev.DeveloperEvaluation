using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponse>
{
    private readonly ISaleRepository _repo;

    public CancelSaleHandler(ISaleRepository repo) => _repo = repo;

    public async Task<CancelSaleResponse> Handle(CancelSaleCommand request, CancellationToken ct)
    {
        var v = new CancelSaleValidator();
        var r = await v.ValidateAsync(request, ct);
        if (!r.IsValid) throw new ValidationException(r.Errors);

        var ok = await _repo.CancelAsync(request.Id, ct);
        if (!ok) throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        return new CancelSaleResponse { Success = true };
    }
}
