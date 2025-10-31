using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<SaleItem, GetSaleItemResult>()
            .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.TotalAmount));
        CreateMap<Sale, GetSaleResult>()
            .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.TotalAmount));
    }
}