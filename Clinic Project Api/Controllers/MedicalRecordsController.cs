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
public sealed class MedicalRecordsController : ControllerBase
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MedicalRecordResponse>>> List(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var list = await _medicalRecordService.ListAsync(userId, roles, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicalRecordResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var item = await _medicalRecordService.GetByIdAsync(id, userId, roles, cancellationToken);
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Doctor}")]
    public async Task<ActionResult<MedicalRecordResponse>> Create([FromBody] CreateMedicalRecordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var created = await _medicalRecordService.CreateAsync(request, userId, roles, cancellationToken);
        return Created($"/api/medicalrecords/{created.Id}", created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Receptionist},{Roles.Doctor}")]
    public async Task<ActionResult<MedicalRecordResponse>> Update(int id, [FromBody] UpdateMedicalRecordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var updated = await _medicalRecordService.UpdateAsync(id, request, userId, roles, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        await _medicalRecordService.DeleteAsync(id, roles, cancellationToken);
        return NoContent();
    }
}
