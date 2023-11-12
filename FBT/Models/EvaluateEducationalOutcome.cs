using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class EvaluateEducationalOutcome
{
    public string SchoolProfileId { get; set; } = null!;

    public string Semester { get; set; } = null!;

    public string AbilitiesAndQualities { get; set; } = null!;

    public string? Level { get; set; }

    public string? Comment { get; set; }

    public virtual SchoolProfile SchoolProfile { get; set; } = null!;
}
