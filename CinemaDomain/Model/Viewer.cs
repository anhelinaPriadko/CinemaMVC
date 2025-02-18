using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Viewer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<FilmRating> FilmRatings { get; set; } = new List<FilmRating>();
}
