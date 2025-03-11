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
            // ������ � ������ ��� ���������: �����, ���������, ��
            var navLinks = new List<(string Text, string Controller, string Action)>
            {
                ("������� ������", "FilmCategories", "Index"),
                ("��������� ������", "Companies", "Index"),
                ("���� ����", "HallTypes", "Index"),
                ("Գ����", "Films", "Index"),
                ("����", "Halls", "Index"),
                ("̳���", "Seats", "Index"),
                ("������", "Sessions", "Index"),
                ("�������", "Viewers", "Index"),
                ("������", "FilmRatings", "Index"),
                ("����������", "Bookings", "Index")
            };

            // �������� � ViewData
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
