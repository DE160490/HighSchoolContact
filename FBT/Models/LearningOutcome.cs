using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class LearningOutcome
{
    public string SchoolProfileId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public decimal? Semester1Scores { get; set; }

    public decimal? Semester2Scores { get; set; }

    public decimal? FinalScores { get; set; }

    public string? ConfirmationsOfSubjectTeacher { get; set; }

    public virtual SchoolProfile SchoolProfile { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
