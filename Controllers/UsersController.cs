using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CurseProject.Models;
using Microsoft.EntityFrameworkCore.Internal;
using CurseProject.ViewModelsl;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CurseProject.Controllers
{
    public class UsersController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataBaseContext _context;

        [Authorize(Roles = "Admin")]
        public IActionResult Index() => View(_userManager.Users.ToList());

        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
        public IActionResult GetClients()
        {
            return View();
        }

        [Authorize(Roles = "Client, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAgents()
        {
            return View("AgentsView", await _userManager.GetUsersInRoleAsync("Agent"));
        }

        [Authorize(Roles = "Agent, Admin")]
        public async Task<IActionResult> GetUsers(string Id, string userName)
        {
            List<User> users = await _userManager.Users.ToListAsync();

            if(Id != null)
            {
                users = users.Where(p => p.Id == Id).ToList();
                return PartialView(users);
            }

            if (userName != null)
            {
                users = users.Where(p => p.UserName == userName).ToList();
            }

            return PartialView(users);
        }

        [Authorize(Roles = "Client, Admin")]
        [HttpPost]
        public async Task<IActionResult> GetAgents(string agentName, string name, string secName, string pathonymic)
        {
            var agents = await _userManager.GetUsersInRoleAsync("Agent");

            if (agentName != null)
            {
                agents = agents.Where(p => p.UserName == agentName).ToList();
            }

            if (name != null)
            {
                agents = agents.Where(p => p.Name == name).ToList();
            }

            if (secName != null)
            {
                agents = agents.Where(p => p.SecName == secName).ToList();
            }

            if (pathonymic != null)
            {
                agents = agents.Where(p => p.Pathonymic == pathonymic).ToList();
            }

            return PartialView(agents);
        }


        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
        public async Task<IActionResult> GetClients(string userName, string passNumber, string change)
        {
            var users = await _userManager.GetUsersInRoleAsync("Client");

            if (change == "0")
            {
                List<Legal_entity> legalEntities = await _context.legal_entity.Include(p => p.user).Where(p => users.Contains(p.user)).ToListAsync();
                if(userName != null)
                {
                    legalEntities = legalEntities.Where(p => p.user.UserName == userName).ToList();
                }

                if(passNumber != null)
                {
                    legalEntities = legalEntities.Where(p => p.user.Passp_number == passNumber).ToList();
                }

                return PartialView("LegalEntities", legalEntities);
            }
            else
            {
                List<User> physPersons = await _context.Users.Where(p => p.legal_entity == null).Where(p => users.Contains(p)).ToListAsync();

                if (userName != null)
                {
                    physPersons = physPersons.Where(p => p.UserName == userName).ToList();
                }

                if (passNumber != null)
                {
                    physPersons = physPersons.Where(p => p.Passp_number == passNumber).ToList();
                }

                return PartialView("PhysPerson", physPersons);
            }
        }

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, DataBaseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> CreateRoles()
        {
            var client = new IdentityRole { Name = "Client" };
            var agent = new IdentityRole { Name = "Agent" };
            var admin = new IdentityRole { Name = "Admin" };
            await _roleManager.CreateAsync(client);
            await _roleManager.CreateAsync(agent);
            await _roleManager.CreateAsync(admin);

            return RedirectToAction("Index", "Contracts");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                User user = new User {
                    Name = model.Name,
                    SecName = model.SecName,
                    Pathonymic = model.Pathonymic, 
                    PhoneNumber = model.TelNumber,
                    Passp_number = model.PassNumber, 
                    Email = model.Email,
                    UserName = model.Email
                };

                if (model.legalEntity)
                {
                    Legal_entity legal_Entity = new Legal_entity
                    {
                        Leg_adress = model.Leg_adress,
                        Paym_account = model.Paym_account,
                        Org_name = model.Org_name,
                    };

                    user.legal_entity = legal_Entity;
                }
                
                var result = await _userManager.CreateAsync(user, model.Password);

                if (model.code == "123")
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Client");
                }

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Contracts");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                false
                );
            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(model.ReturnURL) && Url.IsLocalUrl(model.ReturnURL))
                {
                    return Redirect(model.ReturnURL);
                }
                else
                {
                    return RedirectToAction("index", "Contracts");
                }
            }
            else
            {
                ModelState.AddModelError("", "Пароль или логин введены неверно!");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Contracts");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string Id)
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            List<IdentityRole> Roles = _roleManager.Roles.ToList();

            User user = await _userManager.FindByIdAsync(Id);

            foreach (var a in Roles)
            {
                roles.Add(new SelectListItem
                {
                    Text = a.Name,
                    Value = a.Name
                });
            }

            var model = new ChangeRoleViewModel();
            model.Roles = roles;
            model.user = user;
            IList<String> roleName = await _userManager.GetRolesAsync(user);
            
            model.roleName = roleName[0];
            
            
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string Name, string oldRoleName, string Id)
        {
            User user = await _userManager.FindByIdAsync(Id);

            await _userManager.RemoveFromRoleAsync(user, oldRoleName);

            await _userManager.AddToRoleAsync(user, Name);

            return RedirectToAction("Index");
        }
        

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnURL = returnUrl });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
