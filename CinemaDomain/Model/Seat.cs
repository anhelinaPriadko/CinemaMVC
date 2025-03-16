﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaDomain.Model;

public partial class Seat: Entity
{
    public int HallId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    [Range(1, int.MaxValue, ErrorMessage = "Кількість рядів не повинна бути від'ємною!")]
    public int Row { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    [Range(1, int.MaxValue, ErrorMessage = "Кількість місць не повинна бути від'ємною!")]
    public int NumberInRow { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Hall Hall { get; set; } = null!;
}
