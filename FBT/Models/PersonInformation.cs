using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class PersonInformation
{
    public string Id { get; set; } = null!;

    public string? Fullname { get; set; }

    public bool? Gender { get; set; }

    public DateTime? Dob { get; set; }

    public string? PlaceOfBirth { get; set; }

    public string? PlaceOfResidence { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Ethnic { get; set; }

    public string? Religion { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
