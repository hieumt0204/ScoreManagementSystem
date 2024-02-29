using System;
using System.Collections.Generic;

namespace ScoreManagementSystem.Models;

public partial class Score
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? ComponentScoreId { get; set; }

    public double? Score1 { get; set; }

    public string? Note { get; set; }

    public virtual ComponentScore? ComponentScore { get; set; }

    public virtual ClassStudent? Student { get; set; }
}
