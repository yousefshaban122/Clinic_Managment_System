using System.Security.Claims;
using Clinic.Application.Abstractions;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Project_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Doctor}")]
    public async Task<ActionResult<IReadOnlyList<InvoiceResponse>>> List(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var list = await _invoiceService.ListAsync(userId, roles, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<InvoiceResponse>> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var created = await _invoiceService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Doctor}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var invoice = await _invoiceService.GetByIdAsync(id, userId, roles, cancellationToken);
        return Ok(invoice);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")]
    public async Task<ActionResult<InvoiceResponse>> Update(int id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var updated = await _invoiceService.UpdateAsync(id, request, roles, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        await _invoiceService.DeleteAsync(id, roles, cancellationToken);
        return NoContent();
    }
}
