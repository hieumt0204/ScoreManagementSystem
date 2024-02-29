using System;
using System.Collections.Generic;

namespace ScoreManagementSystem.Models;

public partial class ClassStudent
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public DateTime? JoinDate { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual User? Student { get; set; }
}
