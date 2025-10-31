using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _ctx;
    public SaleRepository(DefaultContext ctx) => _ctx = ctx;

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken ct = default)
    {
        await _ctx.Sales.AddAsync(sale, ct);
        await _ctx.SaveChangesAsync(ct);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<bool> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var sale = await _ctx.Sales.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (sale is null) return false;
        sale.Cancel();
        await _ctx.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken ct = default)
    {
        _ctx.Sales.Update(sale);
        await _ctx.SaveChangesAsync(ct);
        return sale;
    }
}
