using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Teacher
{
    public string TeacherId { get; set; } = null!;

    public string? MainExpertise { get; set; }

    public string? Position { get; set; }

    public virtual Subject? MainExpertiseNavigation { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<SubjectTeacher> SubjectTeachers { get; set; } = new List<SubjectTeacher>();

    public virtual PersonInformation TeacherNavigation { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
