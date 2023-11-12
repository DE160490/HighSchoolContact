using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Grade
{
    public string GradeId { get; set; } = null!;

    public string? GradeName { get; set; }

    public int? MaxClass { get; set; }

    public string? SchoolYearId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual SchoolYear? SchoolYear { get; set; }
}
