﻿using System;
using System.Collections.Generic;

namespace FBT.Models;

public partial class SubjectTeacher
{
    public string TeacherId { get; set; } = null!;

    public string ClassId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
