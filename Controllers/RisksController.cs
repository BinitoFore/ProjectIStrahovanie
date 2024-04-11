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
    [Authorize(Roles = "Agent, Admin")]
    public class RisksController : Controller
    {
        private readonly DataBaseContext _context;

        public RisksController(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return _context.riscs != null ? 
                          View(await _context.riscs.ToListAsync()) :
                          Problem("Entity set 'DataBaseContext.riscs'  is null.");
        }

        public async Task<IActionResult> GetRisks(string riskName)
        {
            List<Risk> risks = await _context.riscs.ToListAsync();

            if (riskName != null)
            {
                risks = risks.Where(p => p.name.ToLower().Contains(riskName.ToLower())).ToList();
            }

            return PartialView(risks);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.riscs == null)
            {
                return NotFound();
            }

            var risk = await _context.riscs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (risk == null)
            {
                return NotFound();
            }

            return View(risk);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,descrip")] Risk risk)
        {
            if (ModelState.IsValid)
            {
                _context.Add(risk);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(risk);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.riscs == null)
            {
                return NotFound();
            }

            var risk = await _context.riscs.FindAsync(id);
            if (risk == null)
            {
                return NotFound();
            }
            return View(risk);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,descrip")] Risk risk)
        {
            if (id != risk.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(risk);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RiskExists(risk.Id))
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
            return View(risk);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.riscs == null)
            {
                return Problem("Entity set 'DataBaseContext.riscs'  is null.");
            }
            var risk = await _context.riscs.FindAsync(id);
            if (risk != null)
            {
                _context.riscs.Remove(risk);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RiskExists(int id)
        {
          return (_context.riscs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
