using System;
using System.Collections.Generic;

namespace ScoreManagementSystem.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

public enum RoleEnum
{
    STUDENT = 1,
    TEACHER = 2,
    ADMIN = 3
}
