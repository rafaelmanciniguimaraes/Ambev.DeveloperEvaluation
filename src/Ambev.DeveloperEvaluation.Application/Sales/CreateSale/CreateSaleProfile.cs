using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
namespace
    Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleItemCommand, SaleItem>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId.ToString()))
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductName))
            .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
            .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice))
            .ForMember(d => d.DiscountPercent, o => o.MapFrom(s => s.DiscountPercent));

        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(d => d.SaleNumber, o => o.MapFrom(s => s.SaleNumber))
            .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CustomerId.ToString()))
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.CustomerName))
            .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId.ToString()))
            .ForMember(d => d.BranchName, o => o.MapFrom(s => s.BranchName))
            .ForMember(d => d.Date, o => o.MapFrom(s => s.Date))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
    }
}
