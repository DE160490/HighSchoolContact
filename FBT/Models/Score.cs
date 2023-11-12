using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Score
{
    public string StudentId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public decimal? Coefficient1 { get; set; }

    public decimal? Coefficient2 { get; set; }

    public decimal? Coefficient3 { get; set; }

    public int Semester { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
