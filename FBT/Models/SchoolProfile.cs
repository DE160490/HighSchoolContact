using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class SchoolProfile
{
    public string SchoolProfileId { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? Reward { get; set; }

    public string? Note { get; set; }

    public string? ConfirmationsOfHomeroomTeacher { get; set; }

    public string? ConfirmationsOfPrincipal { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<EvaluateEducationalOutcome> EvaluateEducationalOutcomes { get; set; } = new List<EvaluateEducationalOutcome>();

    public virtual ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();

    public virtual Student? Student { get; set; }
}
