using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class SchoolYear
{
    public string SchoolYearId { get; set; } = null!;

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
