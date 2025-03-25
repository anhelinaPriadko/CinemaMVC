﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaDomain.Model;
using CinemaInfrastructure;

namespace CinemaInfrastructure.Controllers
{
    public class BookingsController : Controller
    {
        private readonly CinemaContext _context;

        public BookingsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var cinemaContext = _context.Bookings.Include(b => b.Seat)
                .Include(b => b.Seat)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.HallType)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.Company)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.FilmCategory)
                .Include(b => b.Viewer);
            return View(await cinemaContext.ToListAsync());
        }

        public async Task<IActionResult> IndexBySessions(int sessionId)
        {
            var sessionExists = await _context.Sessions.AnyAsync(s => s.Id == sessionId);
            if (!sessionExists)
                return NotFound();

            var bookings = await _context.Bookings.Where(b => b.SessionId == sessionId)
                .Include(b => b.Seat)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.HallType)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.Company)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.FilmCategory)
                .Include(b => b.Viewer)
                .ToListAsync();

            return View("Index", bookings);
        }

        public async Task<IActionResult> IndexByViewers(int viewerId)
        {
            var viewerExists = await _context.Viewers.AnyAsync(s => s.Id == viewerId);
            if (!viewerExists)
                return NotFound();

            var bookings = await _context.Bookings.Where(b => b.ViewerId == viewerId)
                .Include(b => b.Seat)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.HallType)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.Company)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.FilmCategory)
                .Include(b => b.Viewer)
                .ToListAsync();

            if (bookings.Count() == 0)
                TempData["Message"] = "На жаль, у цього користувача ще немає бронювань(";

            return View("Index", bookings);
        }

        public async Task<IActionResult> IndexBySeats(int seatId)
        {
            var seatExists = await _context.Seats.AnyAsync(s => s.Id == seatId);
            if (!seatExists)
                return NotFound();

            var bookings = await _context.Bookings.Where(b => b.SeatId == seatId)
                .Include(b => b.Seat)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.HallType)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.Company)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.FilmCategory)
                .Include(b => b.Viewer)
                .ToListAsync();


            return View("Index", bookings);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? viewerId, int? sessionId, int? seatId)
        {
            if (viewerId == null || sessionId == null || seatId == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Seat)
                    .ThenInclude(s => s.Hall)
                        .ThenInclude(h => h.HallType)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.Company)
                .Include(b => b.Session)
                    .ThenInclude(s => s.Film)
                        .ThenInclude(f => f.FilmCategory)
                .Include(b => b.Viewer)
                .FirstOrDefaultAsync(m =>
                    m.ViewerId == viewerId &&
                    m.SessionId == sessionId &&
                    m.SeatId == seatId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }


        // GET: Bookings/Create
        public IActionResult Create()
        {
            var films = _context.Films.ToList();
            var viewers = _context.Viewers.ToList();
            ViewBag.Films = films;
            ViewData["ViewerId"] = new SelectList(_context.Viewers, "Id", "Name", null);
            return View();
        }


        public async Task<IActionResult> GetSessionsByFilm(int filmId)
        {
            var sessions = await _context.Sessions
                .Where(s => s.Film.Id == filmId)
                .Select(s => new {
                    s.Id,
                    Time = s.SessionTime.ToString("dd.MM.yyyy HH:mm")
                })
                .ToListAsync();
            return Json(sessions);
        }

        public async Task<IActionResult> GetRowsBySession(int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
                return Json(new List<object>());

            var rows = await _context.Seats
                .Where(s => s.Hall.Id == session.Hall.Id)
                .Select(s => s.Row)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync();
            return Json(rows);
        }


        public async Task<IActionResult> GetSeatsByRow(int sessionId, int row)
        {
            var session = await _context.Sessions
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null)
                return Json(new List<object>());

            var seats = await _context.Seats
                .Where(s => s.Hall.Id == session.Hall.Id && s.Row == row)
                .Select(s => new {
                    s.Id,
                    s.NumberInRow
                })
                .OrderBy(s => s.NumberInRow)
                .ToListAsync();
            return Json(seats);
        }

        private bool checkDublication(int sessioId, int seatId)
        {
            return _context.Bookings.Any(b => b.SessionId == sessioId && b.SeatId == seatId);
        }


        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViewerId,SessionId,SeatId")] Booking booking)
        {
            // Завантаження пов’язаних даних
            booking.Viewer = await _context.Viewers.FirstOrDefaultAsync(v => v.Id == booking.ViewerId);
            booking.Session = await _context.Sessions
                .Include(s => s.Hall)
                .Include(s => s.Film)
                .Include(s => s.Film.FilmCategory)
                .Include(s => s.Film.Company)
                .FirstOrDefaultAsync(s => s.Id == booking.SessionId);
            booking.Seat = await _context.Seats
                .Include(s => s.Hall)
                .Include(s => s.Hall.HallType)
                .FirstOrDefaultAsync(s => s.Id == booking.SeatId);

            ModelState.Clear();
            TryValidateModel(booking);

            // Перевірка дублювання бронювання
            if (checkDublication(booking.SessionId, booking.SeatId))
            {
                ModelState.AddModelError("", "Дане місце на цей сеанс вже заброньовано!");
            }

            // Якщо ModelState не валідний, потрібно відновити дані для dropdown’ів
            if (!ModelState.IsValid)
            {
                var films = _context.Films.ToList();
                var viewers = _context.Viewers.ToList();
                ViewBag.Films = films;
                ViewData["ViewerId"] = new SelectList(viewers, "Id", "Name", booking.ViewerId);
                // Для сеансів, рядів та місць можна передбачити значення за замовчуванням
                return View(booking);
            }

            _context.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int viewerId,
    int sessionId,    // початковий сеанс
    int seatId,       // початкове місце
    int newSessionId, // новий сеанс (якщо змінюємо)
    int newSeatId)    // нове місце
        {
            // Знаходимо існуюче бронювання
            var oldBooking = await _context.Bookings.FindAsync(viewerId, sessionId, seatId);
            if (oldBooking == null)
                return NotFound();

            // Перевірка дублювання бронювання (чи зайняте вже нове місце)
            if (_context.Bookings.Any(b => b.SessionId == newSessionId && b.SeatId == newSeatId))
            {
                ModelState.AddModelError("", "Обране місце на цей сеанс вже заброньовано!");
            }

            if (!ModelState.IsValid)
            {
                // Якщо валідація не пройшла, відновлюємо дані для відображення форми
                var sessions = await _context.Sessions
                    .Where(s => s.FilmId == oldBooking.Session.FilmId)
                    .Select(s => new {
                        s.Id,
                        Time = s.SessionTime.ToString("dd.MM.yyyy HH:mm")
                    })
                    .ToListAsync();
                ViewBag.SessionTime = new SelectList(sessions, "Id", "Time", newSessionId);

                // Можна також повторно завантажити ряд та місця (опціонально)
                return View(oldBooking);
            }

            // Оскільки ключі бронювання – композиційні, змінювати їх напряму складно.
            // Тому видаляємо старе бронювання та додаємо нове.
            _context.Bookings.Remove(oldBooking);
            var newBooking = new Booking
            {
                ViewerId = oldBooking.ViewerId,
                SessionId = newSessionId,
                SeatId = newSeatId
            };

            // Завантаження пов'язаних даних (якщо потрібно для подальшої роботи)
            newBooking.Viewer = await _context.Viewers.FirstOrDefaultAsync(v => v.Id == newBooking.ViewerId);
            newBooking.Session = await _context.Sessions
                .Include(s => s.Film)
                .FirstOrDefaultAsync(s => s.Id == newBooking.SessionId);
            newBooking.Seat = await _context.Seats
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == newBooking.SeatId);

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private bool BookingExists(int viewerId, int sessionId, int seatId)
        {
            return _context.Bookings.Any(e =>
                e.ViewerId == viewerId &&
                e.SessionId == sessionId &&
                e.SeatId == seatId
            );
        }


        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int viewerId, int sessionId, int seatId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Seat)
                .Include(b => b.Session)
                .Include(b => b.Viewer)
                .FirstOrDefaultAsync(b =>
                    b.ViewerId == viewerId &&
                    b.SessionId == sessionId &&
                    b.SeatId == seatId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int viewerId, int sessionId, int seatId)
        {
            var booking = await _context.Bookings
                    .Include(b => b.Viewer)
                    .Include(b => b.Session)
                    .Include(b => b.Session.Film)
                    .FirstOrDefaultAsync(b => b.ViewerId == viewerId && 
                    b.SessionId == sessionId && b.SeatId == seatId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Бронювання користувача \"{booking.Viewer.Name}\"" +
                    $" на фільм \"{booking.Session.Film.Name}\"  успішно видалено!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.ViewerId == id);
        }
    }
}
