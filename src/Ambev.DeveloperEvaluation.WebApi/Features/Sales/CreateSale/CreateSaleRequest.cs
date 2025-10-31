﻿using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
public class CreateSaleRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; } 
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; } 
    public string BranchName { get; set; } = string.Empty;
    [Required]
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; } 
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountPercent { get; set; }
}