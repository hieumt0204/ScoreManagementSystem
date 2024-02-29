using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoreManagementSystem.Models;

public partial class User
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public bool? Active { get; set; }

    public bool? Gender { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Class> ClassCreatedByNavigations { get; set; } = new List<Class>();

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Class> ClassTeachers { get; set; } = new List<Class>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
