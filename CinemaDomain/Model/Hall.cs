using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Hall: Entity
{
    public string Name { get; set; } = null!;

    public int NumberOfRows { get; set; }

    public int SeatsInRow { get; set; }

    public int HallTypeId { get; set; }

    public virtual HallType HallType { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
