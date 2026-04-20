using System;
using System.Collections.Generic;

namespace project.Models;

public partial class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // Admin | Staff | Customer

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
