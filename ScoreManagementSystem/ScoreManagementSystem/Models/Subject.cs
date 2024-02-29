using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoreManagementSystem.Models;

public partial class Subject
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }

    public bool? Active { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<ComponentScore> ComponentScores { get; set; } = new List<ComponentScore>();

    public virtual User? CreatedByNavigation { get; set; }
}
