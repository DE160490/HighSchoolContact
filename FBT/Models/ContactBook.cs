using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class ContactBook
{
    public string ContactBookId { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? Content { get; set; }

    public virtual Student? Student { get; set; }
}
