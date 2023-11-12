using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string? Password { get; set; }

    public int? Role { get; set; }

    public virtual PersonInformation AccountNavigation { get; set; } = null!;
}
