using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurseProject.Models;
using Microsoft.AspNetCore.Identity;
using CurseProject.ViewModelsl;
using Microsoft.AspNetCore.Authorization;

namespace CurseProject.Controllers
{
    [Authorize(Roles = "Agent, Admin")]
    public class PropertiesController : Controller
    {
        private readonly DataBaseContext _context;
        private readonly UserManager<User> _userManager;

        public PropertiesController(DataBaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
              return _context.properties != null ? 
                          View(await _context.properties.Include(p => p.owner).ToListAsync()) :
                          Problem("Entity set 'DataBaseContext.properties'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

        
        public async Task<IActionResult> GetProperties(string ownerName, string name)
        {
            List<Property> properties = await _context.properties.
                Include(p => p.contracts).
                Include(p => p.owner).ToListAsync();

            if (ownerName != null)
            {
                properties = properties.Where(p => p.owner.UserName == ownerName).ToList();
            }

            if (name != null)
            {
                properties = properties.Where(p => p.Name.ToLower().Contains(name)).ToList();
            }

            return PartialView(properties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropViewModel model)
        {
            User owner = await _userManager.FindByNameAsync(model.ownerName);
            if (ModelState.IsValid)
            {
                Property property = new Property
                {
                    Name = model.Name,
                    descript = model.descript,
                    owner = owner,
                };
                _context.Add(property);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.properties == null)
            {
                return NotFound();
            }

            var property = await _context.properties.Include(p => p.owner).FirstOrDefaultAsync(p => p.Id == id);

            User owner = await _userManager.FindByIdAsync(property.owner.Id);

            if (property == null)
            {
                return NotFound();
            }

            EditPropViewModel model = new EditPropViewModel
            {
                Id = property.Id,
                Name = property.Name,
                descript = property.descript,
                ownerName = property.owner.UserName,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPropViewModel model)
        {

            if (ModelState.IsValid)
            {
                User owner = await _userManager.FindByNameAsync(model.ownerName);
                
                if (owner == null)
                {
                    model.errorText = "Пользователь не найден в базе данных. Повторите ввод.";
                    return View(model);
                }

                Property property = new Property
                {
                    Id = model.Id,
                    Name = model.Name,
                    descript = model.descript,
                    owner = owner
                };

                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.Id))
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
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.properties == null)
            {
                return NotFound();
            }

            var @property = await _context.properties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.properties == null)
            {
                return Problem("Entity set 'DataBaseContext.properties'  is null.");
            }
            var @property = await _context.properties.FindAsync(id);
            if (@property != null)
            {
                _context.properties.Remove(@property);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
          return (_context.properties?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
