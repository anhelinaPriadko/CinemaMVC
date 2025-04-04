using System;
using System.Collections.Generic;

namespace CinemaInfrastructure.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int HallId { get; set; }

    public int Row { get; set; }

    public int NumberInRow { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hall Hall { get; set; } = null!;
}
