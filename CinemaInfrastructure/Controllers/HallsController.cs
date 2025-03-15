﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaDomain.Model;
using CinemaInfrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace CinemaInfrastructure.Controllers
{
    public class HallsController : Controller
    {
        private readonly CinemaContext _context;

        public HallsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Halls
        public async Task<IActionResult> Index()
        {
            var cinemaContext = _context.Halls.Include(h => h.HallType);
            return View(await cinemaContext.ToListAsync());
        }

        public async Task<IActionResult> IndexByHallType(int hallTypeId)
        {
            var hallTypeExists = await _context.HallTypes.AnyAsync(h => h.Id == hallTypeId);
            if(!hallTypeExists)
            {
                return NotFound();
            }

            var halls = await _context.Halls
                .Where(h => h.HallTypeId == hallTypeId)
                .ToListAsync();

            if(halls.Count() == 0)
                TempData["Message"] = "На жаль, залів цього типу ще немає(";

            return View("Index", halls);
        }

        // GET: Halls/Details/5
        public async Task<IActionResult> DetailsBySeats(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.HallType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return RedirectToAction("IndexByHall", "Seats", new { hallId = hall.Id});
        }

        public async Task<IActionResult> DetailsBySessions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.HallType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return RedirectToAction("IndexByHall", "Sessions", new { hallId = hall.Id });
        }

        public bool CheckNameDublication(string Name)
        {
            return _context.Halls.Any(h => h.Name == Name);
        }

        // GET: Halls/Create
        public IActionResult Create()
        {
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "Id", "Name");
            return View();
        }

        // POST: Halls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,NumberOfRows,SeatsInRow,HallTypeId,Id")] Hall hall)
        {
            HallType hallType = _context.HallTypes.Include(h => h.Halls)
                .FirstOrDefault(h => h.Id == hall.HallTypeId);

            hall.HallType = hallType;
            ModelState.Clear();
            TryValidateModel(hallType);

            if(CheckNameDublication(hall.Name))
            {
                ModelState.AddModelError("Name", "Зал з такою назвою вже існує!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(hall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "Id", "Name", hall.HallTypeId);
            return View(hall);
        }

        // GET: Halls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls.FindAsync(id);

            if (hall == null)
            {
                return NotFound();
            }

            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "Id", "Name", hall.HallTypeId);
            return View(hall);
        }

        // POST: Halls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,NumberOfRows,SeatsInRow,HallTypeId,Id")] Hall hall)
        {
            if (id != hall.Id)
            {
                return NotFound();
            }

            hall.HallType = await _context.HallTypes
                .FindAsync(hall.HallTypeId);

            ModelState.Clear();
            TryValidateModel(hall);

            if (CheckNameDublication(hall.Name))
            {
                ModelState.AddModelError("Name", "Зал з такою назвою вже існує!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HallExists(hall.Id))
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
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "Id", "Name", hall.HallTypeId);
            return View(hall);
        }

        // GET: Halls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.HallType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // POST: Halls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                TempData["ErrorMessage"] = "Зал не знайдено!";
                return RedirectToAction("Index");
            }

            var isLinkedSeats = await _context.Seats.AnyAsync(s => s.HallId == id);
            var isLinkedSessions = await _context.Sessions.AnyAsync(s => s.HallId == id);

            if (isLinkedSeats || isLinkedSessions)
            {
                TempData["ErrorMessage"] = "Цей зал не можна видалити, оскільки він має пов'язані дані!";
                return RedirectToAction("Index");
            }

            _context.Remove(hall);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Зал \"{hall.Name}\" успішно видалено!";
            return RedirectToAction(nameof(Index));
        }

        private bool HallExists(int id)
        {
            return _context.Halls.Any(e => e.Id == id);
        }
    }
}
