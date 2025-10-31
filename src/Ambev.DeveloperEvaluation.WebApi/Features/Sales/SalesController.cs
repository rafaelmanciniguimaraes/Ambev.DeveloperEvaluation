using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken ct)
    {
        var validator = new CreateSaleRequestValidator();
        var vr = await validator.ValidateAsync(request, ct);
        if (!vr.IsValid) return BadRequest(vr.Errors);

        var cmd = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(cmd, ct);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Venda criada com sucesso.",
            Data = _mapper.Map<CreateSaleResponse>(result)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken ct)
    {
        var req = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var vr = await validator.ValidateAsync(req, ct);
        if (!vr.IsValid) return BadRequest(vr.Errors);

        var cmd = new GetSaleCommand(id);
        var res = await _mediator.Send(cmd, ct);

        return Ok(new ApiResponseWithData<GetSaleResponse>
        {
            Success = true,
            Message = "Venda recuperada com sucesso.",
            Data = _mapper.Map<GetSaleResponse>(res)
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken ct)
    {
        var req = new CancelSaleRequest { Id = id };
        var validator = new CancelSaleRequestValidator();
        var vr = await validator.ValidateAsync(req, ct);
        if (!vr.IsValid) return BadRequest(vr.Errors);

        var cmd = _mapper.Map<CancelSaleCommand>(req.Id);
        await _mediator.Send(cmd, ct);

        return Ok(new ApiResponse { Success = true, Message = "Venda cancelada com sucesso." });
    }
}
