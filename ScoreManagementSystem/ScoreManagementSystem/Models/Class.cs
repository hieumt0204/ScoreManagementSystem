using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoreManagementSystem.Models;

public partial class Class
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }

    public bool? Active { get; set; }

    public int? TeacherId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? SubjectId { get; set; }

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual User? Teacher { get; set; }
}
