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
    public class SessionsController : Controller
    {
        private readonly CinemaContext _context;

        public SessionsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            var cinemaContext = _context.Sessions
                .Include(s => s.Film)
                .ThenInclude(f => f.Company)
                .Include(s => s.Film)
                .ThenInclude(f => f.FilmCategory)
                .Include(s => s.Hall)
                .ThenInclude(h => h.HallType);
            return View(await cinemaContext.ToListAsync());
        }

        public async Task<IActionResult> IndexByHall(int hallId)
        {
            var hallExists = await _context.Halls.AnyAsync(h => h.Id == hallId);
            if (!hallExists)
                return NotFound();

            var sessions = await _context.Sessions.Where(s => s.HallId == hallId)
                .Include(s => s.Film)
                .ThenInclude(f => f.Company)
                .Include(s => s.Film)
                .ThenInclude(f => f.FilmCategory)
                .Include(s => s.Hall)
                .ThenInclude(h => h.HallType)
                .ToListAsync();

            if(sessions.Count() == 0)
                TempData["Message"] = "На жаль, в цьому залі ще немає сеансів(";

            return View("Index", sessions);
        }

        public async Task<IActionResult> IndexByFilm(int filmId)
        {
            var filmExists = await _context.Films.AnyAsync(f => f.Id == filmId);
            if (!filmExists)
                return NotFound();

            var sessions = await _context.Sessions.Where(s => s.FilmId == filmId)
                .Include(s => s.Film)
                .ThenInclude(f => f.Company)
                .Include(s => s.Film)
                .ThenInclude(f => f.FilmCategory)
                .Include(s => s.Hall)
                .ThenInclude(h => h.HallType)
                .ToListAsync();

            if (sessions.Count() == 0)
                TempData["Message"] = "На жаль, для цього фільму ще немає сеансів(";

            return View("Index", sessions);
        }


        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Film)
                .ThenInclude(f => f.Company)
                .Include(s => s.Film)
                .ThenInclude(f => f.FilmCategory)
                .Include(s => s.Hall)
                .ThenInclude(h => h.HallType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        public async Task<IActionResult> DetailsByBookings(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Film)
                .ThenInclude(f => f.Company)
                .Include(s => s.Film)
                .ThenInclude(f => f.FilmCategory)
                .Include(s => s.Hall)
                .ThenInclude(h => h.HallType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return RedirectToAction("IndexBySessions", "Bookings", new { sessionId = session.Id});
        }


        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name");
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name");
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmId,HallId,SessionTime,Duration,Id")] Session session)
        {
            Hall hall  = await _context.Halls
                .Include(h => h.HallType)
                .FirstOrDefaultAsync(h => h.Id == session.HallId);

            Film film = await _context.Films
                .Include(f => f.FilmCategory)
                .Include(f => f.Company)
                .FirstOrDefaultAsync(f => f.Id == session.FilmId);

            session.Hall = hall;
            session.Film = film;

            ModelState.Clear();
            TryValidateModel(session);

            if (ModelState.IsValid)
            {
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", session.FilmId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", session.FilmId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmId,HallId,SessionTime,Duration,Id")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            Film film = await _context.Films
                .Include(f => f.Company)
                .Include(f => f.FilmCategory)
                .FirstOrDefaultAsync(f => f.Id == session.FilmId);

            Hall hall = await _context.Halls
                .Include(h => h.HallType)
                .FirstOrDefaultAsync(h => h.Id == session.HallId);

            session.Hall = hall;
            session.Film = film;

            ModelState.Clear();
            TryValidateModel(session);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmId"] = new SelectList(_context.Films, "Id", "Name", session.FilmId);
            ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Film)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions
                                .Include(s => s.Film)
                                .FirstOrDefaultAsync(s => s.Id == id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Сеанс не знайдено!";
                return RedirectToAction("Index");
            }

            var isLinked = await _context.Bookings.AnyAsync(b => b.SessionId == id);
            if(isLinked)
            {
                TempData["ErrorMessage"] = "Цей сеанс не можна видалити, оскільки він має пов'язані дані!";
                return RedirectToAction("Index");
            }

            string filmName = session.Film.Name;
            string filmDateTime = session.SessionTime.ToString("dd.MM.yyyy HH.mm");
            _context.Remove(session);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Сеанс фільму \"{filmName}\" \"{filmDateTime}\" успішно видалено!";
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
        }
    }
}
