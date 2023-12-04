using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBT.Models;

public partial class PersonInformation
{
    public string Id { get; set; } = null!;

    [Required(ErrorMessage = "Bắt buộc nhập Họ và Tên.")]
    [RegularExpression(@"^[\p{L}\s]*$", ErrorMessage = "Học và tên chỉ bao gồm chữ (Không có cách đầu )")]
    public string? Fullname { get; set; }

    public bool? Gender { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập Ngày tháng năm sinh.")]
    public DateTime? Dob { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập Nơi Sinh.")]
    public string? PlaceOfBirth { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập Nơi ở hiện tại.")]
    public string? PlaceOfResidence { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập SĐT.")]
    [RegularExpression(@"^[0 ][1-9 ][0-9 ]{8}$", ErrorMessage = "SĐT phải là số (Gồm 10 số và bắt đầu là 0)")]
    public string? Phone { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email phải đúng format (abc@gmail.com)")]
    [Required(ErrorMessage = "Bắt buộc nhập Email.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập Dân tộc.")]
    [RegularExpression(@"^[\p{L}\s]*$", ErrorMessage = "Dân tộc chỉ bao gồm chữ")]
    public string? Ethnic { get; set; }

    [Required(ErrorMessage = "Bắt buộc nhập Tôn giáo.")]
    [RegularExpression(@"^[\p{L}\s]*$", ErrorMessage = "Tôn giáo chỉ bao gồm chữ")]
    public string? Religion { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
