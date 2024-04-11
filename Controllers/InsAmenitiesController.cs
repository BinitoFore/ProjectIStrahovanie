using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurseProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace CurseProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InsAmenitiesController : Controller
    {
        private readonly DataBaseContext _context;

        public InsAmenitiesController(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return _context.insAmenities != null ? 
                          View(await _context.insAmenities.ToListAsync()) :
                          Problem("Entity set 'DataBaseContext.insAmenities'  is null.");
        }

        public async Task<IActionResult> GetInsAmenities(string amenitName)
        {
            if (amenitName == null)
            {
                amenitName = "";
            }
            
            return PartialView(await _context.insAmenities.Where(p => p.Name.ToLower().Contains(amenitName.ToLower())).ToListAsync());
            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.insAmenities == null)
            {
                return NotFound();
            }

            var insAmenities = await _context.insAmenities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insAmenities == null)
            {
                return NotFound();
            }

            return View(insAmenities);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,descript")] InsAmenities insAmenities)
        {
            if (ModelState.IsValid)
            {
                _context.Add(insAmenities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(insAmenities);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.insAmenities == null)
            {
                return NotFound();
            }

            var insAmenities = await _context.insAmenities.FindAsync(id);
            if (insAmenities == null)
            {
                return NotFound();
            }
            return View(insAmenities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,descript")] InsAmenities insAmenities)
        {
            if (id != insAmenities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(insAmenities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsAmenitiesExists(insAmenities.Id))
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
            return View(insAmenities);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.insAmenities == null)
            {
                return Problem("Entity set 'DataBaseContext.insAmenities'  is null.");
            }
            var insAmenities = await _context.insAmenities.FindAsync(id);
            if (insAmenities != null)
            {
                _context.insAmenities.Remove(insAmenities);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InsAmenitiesExists(int id)
        {
          return (_context.insAmenities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
