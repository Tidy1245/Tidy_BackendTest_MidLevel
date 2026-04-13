using Microsoft.AspNetCore.Mvc;
using Tidy_BackendTest_MidLevel.Application.DTOs;
using Tidy_BackendTest_MidLevel.Application.Services;

namespace Tidy_BackendTest_MidLevel.API.Controllers;

/// <summary>
/// MyOffice 人員帳號資料 CRUD API
/// </summary>
[ApiController]
[Route("api/myofficeacpd")]
[Produces("application/json")]
public class MyOfficeAcpdController : ControllerBase
{
    private readonly MyOfficeAcpdService _service;
    private readonly ILogger<MyOfficeAcpdController> _logger;

    public MyOfficeAcpdController(MyOfficeAcpdService service, ILogger<MyOfficeAcpdController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 查詢所有人員帳號資料
    /// </summary>
    /// <returns>人員帳號資料清單</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MyOfficeAcpdDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢所有人員帳號資料時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤", detail = ex.Message });
        }
    }

    /// <summary>
    /// 依 SID 查詢單筆人員帳號資料
    /// </summary>
    /// <param name="id">人員 SID（20 字元）</param>
    /// <returns>人員帳號資料</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MyOfficeAcpdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null)
                return NotFound(new { message = $"ACPD_SID '{id}' 不存在" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢人員帳號資料時發生錯誤，SID: {Id}", id);
            return StatusCode(500, new { message = "伺服器內部錯誤", detail = ex.Message });
        }
    }

    /// <summary>
    /// 新增人員帳號資料
    /// </summary>
    /// <remarks>
    /// 範例請求：
    ///
    ///     POST /api/myofficeacpd
    ///     {
    ///       "ACPD_Cname": "王小明",
    ///       "ACPD_Ename": "Wang Xiao Ming",
    ///       "ACPD_Sname": "WangXM",
    ///       "ACPD_Email": "wang@myoffice.com",
    ///       "ACPD_Status": 0,
    ///       "ACPD_Stop": false,
    ///       "ACPD_StopMemo": "",
    ///       "ACPD_LoginID": "wangxm01",
    ///       "ACPD_LoginPWD": "P@ssw0rd123",
    ///       "ACPD_Memo": "測試帳號"
    ///     }
    /// </remarks>
    /// <param name="request">新增資料</param>
    /// <returns>已建立的人員帳號資料</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MyOfficeAcpdDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateMyOfficeAcpdRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.ACPD_SID }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增人員帳號資料時發生錯誤");
            return StatusCode(500, new { message = "伺服器內部錯誤", detail = ex.Message });
        }
    }

    /// <summary>
    /// 依 SID 更新人員帳號資料
    /// </summary>
    /// <remarks>
    /// 範例請求：
    ///
    ///     PUT /api/myofficeacpd/{id}
    ///     {
    ///       "ACPD_Cname": "王小明（更新）",
    ///       "ACPD_Ename": "Wang Xiao Ming Updated",
    ///       "ACPD_Email": "wang.updated@myoffice.com",
    ///       "ACPD_Status": 1,
    ///       "ACPD_Stop": false,
    ///       "ACPD_LoginPWD": "NewP@ssw0rd456",
    ///       "ACPD_Memo": "更新後備註"
    ///     }
    /// </remarks>
    /// <param name="id">人員 SID（20 字元）</param>
    /// <param name="request">更新資料</param>
    /// <returns>更新後的人員帳號資料</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MyOfficeAcpdDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateMyOfficeAcpdRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"ACPD_SID '{id}' 不存在" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新人員帳號資料時發生錯誤，SID: {Id}", id);
            return StatusCode(500, new { message = "伺服器內部錯誤", detail = ex.Message });
        }
    }

    /// <summary>
    /// 依 SID 刪除人員帳號資料
    /// </summary>
    /// <param name="id">人員 SID（20 字元）</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"ACPD_SID '{id}' 不存在" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除人員帳號資料時發生錯誤，SID: {Id}", id);
            return StatusCode(500, new { message = "伺服器內部錯誤", detail = ex.Message });
        }
    }
}
