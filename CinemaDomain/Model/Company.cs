using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Company: Entity
{
    public string Name { get; set; } = null!;

    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
