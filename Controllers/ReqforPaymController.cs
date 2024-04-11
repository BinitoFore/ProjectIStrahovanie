using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CurseProject.Models;
using NuGet.ProjectModel;
using CurseProject.ViewModelsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CurseProject.Controllers
{
    [Authorize]
    public class ReqforPaymController : Controller
    {
        private readonly DataBaseContext _context;

        public ReqforPaymController(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Agent") || User.IsInRole("Admin"))
            {
                return _context.req_For_Payms != null ?
                            View(await _context.req_For_Payms.Include(p => p.contract.client).ToListAsync()) :
                            Problem("Entity set 'DataBaseContext.req_For_Payms'  is null.");
            }
            else
            {
                return _context.req_For_Payms != null ?
                          View(await _context.req_For_Payms.Include(p => p.contract.client).
                          Where(p => p.contract.client.UserName == User.Identity.Name).ToListAsync()) :
                          Problem("Entity set 'DataBaseContext.req_For_Payms'  is null.");
            }
        }

        public async Task<IActionResult> GetReqForPaym(string userName, string num_of_contr)
        {
            List<Req_for_paym> reqForPaym = await _context.req_For_Payms.
                Include(p => p.contract.client).ToListAsync();

            if (!User.IsInRole("Agent") && !User.IsInRole("admin"))
            {
                reqForPaym = reqForPaym.Where(p => p.contract.client.UserName == User.Identity.Name).ToList();
            }

            if (num_of_contr != null)
            {
                reqForPaym = reqForPaym.Where(p => p.contract.num_of_contr.ToString() == num_of_contr).ToList();
                return PartialView(reqForPaym);
            }

            if (userName != null)
            {
                reqForPaym = reqForPaym.Where(p => p.contract.client.UserName == userName).ToList();
            }

            return PartialView(reqForPaym);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.req_For_Payms == null)
            {
                return NotFound();
            }

            var req_for_paym = await _context.req_For_Payms.Include(p => p.contract.client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (req_for_paym == null)
            {
                return NotFound();
            }

            return View(req_for_paym);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            ReqForPaymModel model = new ReqForPaymModel
            {
                id = id
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReqForPaymModel model)
        {
            Contract contract = await _context.Contracts.FindAsync(model.id);

            contract.Is_problem = true;

            Req_for_paym reqForPaym = new Req_for_paym
            {
                contract = contract,
                description = model.description
            };

            _context.Add(reqForPaym);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var req_for_paym = await _context.req_For_Payms.Include(p => p.contract).FirstOrDefaultAsync(p => p.Id == id);
            req_for_paym.contract.Is_problem = false;
            _context.Remove(req_for_paym);

            if (req_for_paym != null)
            {
                _context.req_For_Payms.Remove(req_for_paym);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
