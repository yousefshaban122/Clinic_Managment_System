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
public sealed class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DoctorResponse>>> List(CancellationToken cancellationToken)
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var list = await _doctorService.ListAsync(roles, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        return Ok(doctor);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<DoctorResponse>> Create([FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var created = await _doctorService.CreateAsync(request, cancellationToken);
        return Created($"/api/doctors/{created.Id}", created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Doctor}")]
    public async Task<ActionResult<DoctorResponse>> Update(int id, [FromBody] UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var updated = await _doctorService.UpdateAsync(id, request, userId, roles, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        await _doctorService.DeleteAsync(id, roles, cancellationToken);
        return NoContent();
    }

    [HttpGet("{doctorId:int}/appointments")]
    public async Task<ActionResult> GetDoctorAppointments(int doctorId, [FromServices] IAppointmentService appointments, CancellationToken cancellationToken)
    {
        if (User.IsInRole(Roles.Doctor))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            var linkedDoctorId = await _doctorService.GetDoctorIdForUserAsync(userId, cancellationToken);
            if (linkedDoctorId != doctorId)
                return Forbid();
        }

        var list = await appointments.GetByDoctorAsync(doctorId, cancellationToken);
        return Ok(list);
    }
}
