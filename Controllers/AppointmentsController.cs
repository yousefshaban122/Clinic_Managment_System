using System.Security.Claims;
using Clinic.Application.Abstractions;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Project_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Doctor}")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponse>>> List(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("uid");
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        var list = await _appointmentService.ListAsync(userId, roles, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("uid");
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        var item = await _appointmentService.GetByIdAsync(id, userId, roles, cancellationToken);
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create([FromBody] CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("uid");
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        var created = await _appointmentService.CreateAsync(request, userId, roles, cancellationToken);
        return Created($"/api/appointments/{created.Id}", created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist}")]
    public async Task<ActionResult<AppointmentResponse>> Update(int id, [FromBody] UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        var updated = await _appointmentService.UpdateAsync(id, request, roles, cancellationToken);
        return Ok(updated);
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<AppointmentResponse>> UpdateStatus(
        int id,
        [FromBody] UpdateAppointmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("uid");
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        var updated = await _appointmentService.UpdateStatusAsync(id, request, userId, roles, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue("uid");
        var roles = User.FindAll("role").Select(c => c.Value).ToList();
        await _appointmentService.DeleteAsync(id, userId, roles, cancellationToken);
        return NoContent();
    }
}
