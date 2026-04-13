using Clinic.Application.Abstractions;
using Clinic.Application.DTOs;
using Clinic.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Project_Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = Roles.Admin)]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IUserAdminService _userAdminService;

    public AdminUsersController(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdminUserResponse>>> List(CancellationToken cancellationToken)
    {
        var list = await _userAdminService.ListAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AdminUserResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var user = await _userAdminService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<AdminUserResponse>> Create([FromBody] AdminCreateUserRequest request, CancellationToken cancellationToken)
    {
        var created = await _userAdminService.CreateAsync(request, cancellationToken);
        return Created($"/api/admin/users/{created.Id}", created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AdminUserResponse>> Update(string id, [FromBody] AdminUpdateUserRequest request, CancellationToken cancellationToken)
    {
        var updated = await _userAdminService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _userAdminService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id}/roles")]
    public async Task<ActionResult<AdminUserResponse>> SetRoles(string id, [FromBody] AdminSetRolesRequest request, CancellationToken cancellationToken)
    {
        var updated = await _userAdminService.SetRolesAsync(id, request, cancellationToken);
        return Ok(updated);
    }
}
