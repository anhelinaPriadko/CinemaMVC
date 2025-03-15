using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaDomain.Model;

public partial class Company: Entity
{
    [Required(ErrorMessage = "Введіть назву виробника!")]
    [StringLength(50, ErrorMessage = "Назва виробника не може перевищувати 50 символів!")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Film> Films { get; set; } = new List<Film>();
}
