using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Schedule
{
    public string ScheduleId { get; set; } = null!;

    public string? TeacherId { get; set; }

    public string? ClassId { get; set; }

    public string? SubjectId { get; set; }

    public DateTime? WeekBegins { get; set; }

    public DateTime? WeekEnds { get; set; }

    public DateTime? DayOfWeek { get; set; }

    public string? Lecture { get; set; }

    public TimeSpan? TimeStart { get; set; }

    public TimeSpan? TimeEnd { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
