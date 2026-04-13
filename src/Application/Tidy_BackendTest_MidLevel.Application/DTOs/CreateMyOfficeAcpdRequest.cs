using System.ComponentModel.DataAnnotations;

namespace Tidy_BackendTest_MidLevel.Application.DTOs;

public class CreateMyOfficeAcpdRequest
{
    /// <summary>中文姓名</summary>
    /// <example>王小明</example>
    [StringLength(60)]
    public string? ACPD_Cname { get; set; }

    /// <summary>英文姓名</summary>
    /// <example>Wang Xiao Ming</example>
    [StringLength(40)]
    public string? ACPD_Ename { get; set; }

    /// <summary>簡稱</summary>
    /// <example>WangXM</example>
    [StringLength(40)]
    public string? ACPD_Sname { get; set; }

    /// <summary>Email</summary>
    /// <example>wang@myoffice.com</example>
    [StringLength(60)]
    [EmailAddress]
    public string? ACPD_Email { get; set; }

    /// <summary>狀態 (0=正常, 1=停用)</summary>
    /// <example>0</example>
    public byte? ACPD_Status { get; set; }

    /// <summary>停用旗標</summary>
    /// <example>false</example>
    public bool? ACPD_Stop { get; set; }

    /// <summary>停用備註</summary>
    /// <example></example>
    [StringLength(60)]
    public string? ACPD_StopMemo { get; set; }

    /// <summary>登入帳號</summary>
    /// <example>wangxm01</example>
    [Required]
    [StringLength(30)]
    public string ACPD_LoginID { get; set; } = string.Empty;

    /// <summary>登入密碼</summary>
    /// <example>P@ssw0rd123</example>
    [Required]
    [StringLength(60)]
    public string ACPD_LoginPWD { get; set; } = string.Empty;

    /// <summary>備註</summary>
    /// <example>測試帳號</example>
    [StringLength(600)]
    public string? ACPD_Memo { get; set; }
}
