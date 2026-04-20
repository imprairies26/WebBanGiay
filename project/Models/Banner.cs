using System;
using System.Collections.Generic;

namespace project.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}
