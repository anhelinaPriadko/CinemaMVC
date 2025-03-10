using System;
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
                .Include(b => b.Session)
                .Include(b => b.Session.Film)
                .Include(b => b.Viewer);
            return View(await cinemaContext.ToListAsync());
        }

        public async Task<IActionResult> IndexBySessions(int sessionId)
        {
            var sessionExists = await _context.Sessions.AnyAsync(s => s.Id == sessionId);
            if (!sessionExists)
                return NotFound();

            var bookings = await _context.Bookings.Where(b => b.SessionId == sessionId)
                .Include(b => b.Seat).Include(b => b.Session).Include(b => b.Viewer)
                .ToListAsync();

            return View("Index", bookings);
        }

        public async Task<IActionResult> IndexByViewers(int viewerId)
        {
            var viewerExists = await _context.Viewers.AnyAsync(s => s.Id == viewerId);
            if (!viewerExists)
                return NotFound();

            var bookings = await _context.Bookings.Where(b => b.ViewerId == viewerId)
                .Include(b => b.Seat).Include(b => b.Session).Include(b => b.Viewer)
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
                .Include(b => b.Seat).Include(b => b.Session).Include(b => b.Viewer)
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
        // GET: Bookings/Create
        public IActionResult Create()
        {
            // Завантажуємо список фільмів із бази даних
            var films = _context.Films.ToList();
            // Передаємо список у ViewBag, щоб у View використовувати його для формування випадаючого списку
            ViewBag.Films = films;

            // Якщо потрібно – завантажуємо дані для інших списків
            ViewData["ViewerId"] = new SelectList(_context.Viewers, "Id", "Name");
            // Для сеансів та місць залишаємо порожні або формуємо їх через AJAX
            return View();
        }

        public async Task<IActionResult> GetSessionsByFilm(int filmId)
        {
            // Отримуємо сеанси для обраного фільму
            var sessions = await _context.Sessions
                .Where(s => s.Film.Id == filmId)
                .Select(s => new {
                    s.Id,
                    Time = s.SessionTime.ToString("dd.MM HH:mm")
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





        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ViewerId,SessionId,SeatId")] Booking booking)
        {
            Viewer viewer = await _context.Viewers.FirstOrDefaultAsync(v => v.Id == booking.ViewerId);

            Session session = await _context.Sessions
                .Include(s => s.Hall)
                .Include(s => s.Film)
                .Include(s => s.Film.FilmCategory)
                .Include(s => s.Film.Company)
                .FirstOrDefaultAsync(s => s.Id == booking.SessionId);

            Seat seat = await _context.Seats
                .Include(s => s.Hall)
                .Include(s => s.Hall.HallType)
                .FirstOrDefaultAsync(s => s.Id == booking.SeatId);

            booking.Viewer = viewer;
            booking.Session = session;
            booking.Seat = seat;

            ModelState.Clear();
            TryValidateModel(booking);

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Id", booking.SeatId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id", booking.SessionId);
            ViewData["ViewerId"] = new SelectList(_context.Viewers, "Id", "Name", booking.ViewerId);
            return View(booking);
        }

        // GET: Bookings/Edit
        public async Task<IActionResult> Edit(int viewerId, int sessionId, int seatId)
{
    var booking = await _context.Bookings
        .Include(b => b.Seat)
        .Include(b => b.Session)
            .ThenInclude(s => s.Film)
        .FirstOrDefaultAsync(b =>
            b.ViewerId == viewerId &&
            b.SessionId == sessionId &&
            b.SeatId == seatId);

    if (booking == null)
        return NotFound();

    // Завантажуємо список всіх фільмів
    ViewBag.Films = _context.Films.ToList();

    // Завантажуємо сеанси для того фільму, який прив’язаний до цього бронювання
    ViewBag.Sessions = _context.Sessions
        .Where(s => s.FilmId == booking.Session.FilmId)
        .Select(s => new {
            s.Id,
            SessionTime = s.SessionTime.ToString("dd.MM HH:mm")
        })
        .ToList();

    // Можна не завантажувати rows/seats – вони будуть отримуватися через AJAX

    return View(booking);
}




        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
                int viewerId,
                int sessionId, // Старий SessionId
                int seatId,    // Старий SeatId
                int newSessionId, // Новий SessionId
                int newSeatId     // Новий SeatId
                )
        {
            var oldBooking = await _context.Bookings.FindAsync(viewerId, sessionId, seatId);
            if (oldBooking == null)
                return NotFound();

            // Видаляємо старе бронювання
            _context.Bookings.Remove(oldBooking);

            // Створюємо нове бронювання з новими даними
            var newBooking = new Booking
            {
                ViewerId = oldBooking.ViewerId,
                SessionId = newSessionId, // Оновлений сеанс
                SeatId = newSeatId        // Оновлене місце
            };

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
            var booking = await _context.Bookings.FindAsync(viewerId, sessionId, seatId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.ViewerId == id);
        }
    }
}
