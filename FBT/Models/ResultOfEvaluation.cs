using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class ResultOfEvaluation
{
    public string? SchoolProfileId { get; set; }

    public string? Semester1Evaluation { get; set; }

    public string? Semester2Evaluation { get; set; }

    public string? FinalEvaluation { get; set; }

    public virtual SchoolProfile? SchoolProfile { get; set; }
}
