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
    public class ViewersController : Controller
    {
        private readonly CinemaContext _context;

        public ViewersController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Viewers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Viewers.ToListAsync());
        }

        // GET: Viewers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewer = await _context.Viewers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viewer == null)
            {
                return NotFound();
            }

            return View(viewer);
        }

        // GET: Viewers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Viewers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DateOfBirth,Id")] Viewer viewer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewer);
        }

        // GET: Viewers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewer = await _context.Viewers.FindAsync(id);
            if (viewer == null)
            {
                return NotFound();
            }
            return View(viewer);
        }

        // POST: Viewers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,DateOfBirth,Id")] Viewer viewer)
        {
            if (id != viewer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViewerExists(viewer.Id))
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
            return View(viewer);
        }

        // GET: Viewers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewer = await _context.Viewers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viewer == null)
            {
                return NotFound();
            }

            return View(viewer);
        }

        // POST: Viewers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viewer = await _context.Viewers.FindAsync(id);
            if (viewer != null)
            {
                _context.Viewers.Remove(viewer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViewerExists(int id)
        {
            return _context.Viewers.Any(e => e.Id == id);
        }
    }
}
