using System;
using System.Collections.Generic;

namespace CinemaInfrastructure.Models;

public partial class HallType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
