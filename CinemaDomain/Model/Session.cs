using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Session: Entity
{
    public int FilmId { get; set; }

    public int HallId { get; set; }

    public DateTime SessionTime { get; set; }

    public int Duration { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Film Film { get; set; } = null!;

    public virtual Hall Hall { get; set; } = null!;
}
