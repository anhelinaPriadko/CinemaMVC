using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Booking
{
    public int ViewerId { get; set; }

    public int SessionId { get; set; }

    public int SeatId { get; set; }

    public virtual Seat Seat { get; set; } = null!;

    public virtual Session Session { get; set; } = null!;

    public virtual Viewer Viewer { get; set; } = null!;
}
