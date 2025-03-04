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
            var cinemaContext = _context.Bookings.Include(b => b.Seat).Include(b => b.Session).Include(b => b.Viewer);
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

            if(bookings.Count() == 0)
                TempData["Message"] = "На жаль, на цей сеанс ще немає бронювань(";

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

            if (bookings.Count() == 0)
                TempData["Message"] = "На жаль, це місце ще не заброньоване на жоден сеанс(";

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
            ViewData["SeatId"] = new SelectList(_context.Seats, "Id", "Id");
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id");
            ViewData["ViewerId"] = new SelectList(_context.Viewers, "Id", "Name");
            return View();
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
                .FirstOrDefaultAsync(b =>
                    b.ViewerId == viewerId &&
                    b.SessionId == sessionId &&
                    b.SeatId == seatId);

            if (booking == null)
                return NotFound();

            ViewBag.Seats = await _context.Seats.ToListAsync();

            return View(booking);
        }



        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int viewerId,
    int sessionId,
    int seatId,
    int newSeatId // Це ім'я поля, яке ми передали через <select name="NewSeatId" 
)
        {
            var oldBooking = await _context.Bookings.FindAsync(viewerId, sessionId, seatId);
            if (oldBooking == null)
                return NotFound();

            _context.Bookings.Remove(oldBooking);

            var newBooking = new Booking
            {
                ViewerId = oldBooking.ViewerId,
                SessionId = oldBooking.SessionId,
                SeatId = newSeatId
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
