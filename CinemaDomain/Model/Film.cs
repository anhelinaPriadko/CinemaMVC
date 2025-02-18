using System;
using System.Collections.Generic;

namespace CinemaDomain.Model;

public partial class Film: Entity
{
    public int CompanyId { get; set; }

    public int FilmCategoryId { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public string? Description { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual FilmCategory FilmCategory { get; set; } = null!;

    public virtual ICollection<FilmRating> FilmRatings { get; set; } = new List<FilmRating>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
