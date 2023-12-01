using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBT.Models;

public partial class Account
{
    [Required]
    public string AccountId { get; set; } = null!;

    [Required]
    public string? Password { get; set; }

    public int? Role { get; set; }

    public virtual PersonInformation? AccountNavigation { get; set; }
}
