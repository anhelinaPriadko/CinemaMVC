using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class FilmRating
{
    public int Id { get; set; }

    public int ViewerId { get; set; }

    public int FilmId { get; set; }

    public int? Rating { get; set; }

    public virtual Film Film { get; set; } = null!;

    public virtual Viewer Viewer { get; set; } = null!;
}
