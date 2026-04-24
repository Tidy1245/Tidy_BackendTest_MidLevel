using Microsoft.AspNetCore.Mvc;
using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Services;

namespace Tidy_BackendTest_MidLevel.API.Controllers;

/// <summary>
/// 租戶管理 API
/// </summary>
[ApiController]
[Route("api/tenants")]
[Produces("application/json")]
public class TenantController : ControllerBase
{
    private readonly TenantService _tenantService;
    private readonly ILogger<TenantController> _logger;

    public TenantController(TenantService tenantService, ILogger<TenantController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢所有租戶
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _tenantService.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢所有租戶時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 依 TenantId 查詢單筆租戶
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        try
        {
            var result = await _tenantService.GetByIdAsync(id);
            if (result is null) return NotFound($"TenantId '{id}' 不存在");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢租戶 {TenantId} 時發生錯誤", id);
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 新增租戶（TenantId 自動產生）
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request)
    {
        try
        {
            var result = await _tenantService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.TenantId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增租戶時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 依 TenantId 更新租戶
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            var result = await _tenantService.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新租戶 {TenantId} 時發生錯誤", id);
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 依 TenantId 刪除租戶
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        try
        {
            await _tenantService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除租戶 {TenantId} 時發生錯誤", id);
            return StatusCode(500, "伺服器內部錯誤");
        }
    }
}
