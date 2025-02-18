using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class HallType: Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
