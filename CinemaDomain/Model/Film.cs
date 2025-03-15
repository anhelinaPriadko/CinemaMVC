﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaDomain.Model;

public partial class Film : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    [StringLength(30, ErrorMessage = "Назва виробника не може перевищувати 30 символів!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    public int CompanyId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    public int FilmCategoryId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
    [FutureDate("1896-01-25")] 
    public DateOnly ReleaseDate { get; set; }

    public string? Description { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual FilmCategory FilmCategory { get; set; } = null!;

    public virtual ICollection<FilmRating> FilmRatings { get; set; } = new List<FilmRating>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}

public class FutureDateAttribute : ValidationAttribute
{
    private readonly DateOnly _minDate;

    public FutureDateAttribute(string minDate)
    {
        _minDate = DateOnly.Parse(minDate);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly dateValue && dateValue <= _minDate)
        {
            return new ValidationResult($"Дата повинна бути пізнішою за {_minDate:dd.MM.yyyy}!");
        }
        return ValidationResult.Success;
    }
}
