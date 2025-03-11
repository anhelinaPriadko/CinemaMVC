using CinemaInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CinemaInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult IndexLinks()
        {
            // Список з даними про посилання: текст, контролер, дія
            var navLinks = new List<(string Text, string Controller, string Action)>
            {
                ("Категорії фільмів", "FilmCategories", "Index"),
                ("Виробники фільмів", "Companies", "Index"),
                ("Типи залів", "HallTypes", "Index"),
                ("Фільми", "Films", "Index"),
                ("Зали", "Halls", "Index"),
                ("Місця", "Seats", "Index"),
                ("Сеанси", "Sessions", "Index"),
                ("Глядачі", "Viewers", "Index"),
                ("Оцінки", "FilmRatings", "Index"),
                ("Бронювання", "Bookings", "Index")
            };

            // Зберігаємо у ViewData
            ViewData["NavLinks"] = navLinks;

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
