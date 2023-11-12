using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public virtual ICollection<ContactBook> ContactBooks { get; set; } = new List<ContactBook>();

    public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();

    public virtual ICollection<SchoolProfile> SchoolProfiles { get; set; } = new List<SchoolProfile>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual PersonInformation StudentNavigation { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
