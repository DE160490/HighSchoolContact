using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Subject
{
    public string SubjectId { get; set; } = null!;

    public string? SubjectName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual ICollection<SubjectTeacher> SubjectTeachers { get; set; } = new List<SubjectTeacher>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public virtual ICollection<Teacher> TeachersNavigation { get; set; } = new List<Teacher>();
}
