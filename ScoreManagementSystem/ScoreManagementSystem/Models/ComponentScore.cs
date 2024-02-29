using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoreManagementSystem.Models;

public partial class ComponentScore
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public double? Percent { get; set; }

    public bool? Active { get; set; }

    public string? Description { get; set; }

    public int? SubjectId { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual Subject? Subject { get; set; }
}
