using CurseProject.Models;
using CurseProject.ViewModelsl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace CurseProject.Controllers
{
	public class ContractsController : Controller
	{
		private readonly DataBaseContext _Context;
		private readonly UserManager<User> _userManager;
		
		public async Task<IActionResult> Index()
		{

			if (User.IsInRole("Agent") || User.IsInRole("Admin"))
			{
                return View(_Context.Contracts.Include(c => c.client).
							Include(c => c.agent).
							Include(p => p.risk).
							Include(p => p.insAmenities).
							Where(p => p.Is_problem).ToList());
            }
			else
			{
                return View(_Context.Contracts.Include(c => c.client).
                            Include(c => c.agent).
                            Include(p => p.risk).
                            Include(p => p.insAmenities).
                            Where(p => p.Is_problem).
							Where(p => p.client.UserName == User.Identity.Name).ToList());
            }
		}

		public ContractsController(DataBaseContext _context, UserManager<User> userMeneger)
		{
			_Context = _context;
			_userManager = userMeneger;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> GetContracts(string type, string userType, string contrType, string clientName, string agentName, DateTime startDate, DateTime endDate)
		{
			List<Contract> contracts = await _Context.Contracts.Include(p => p.client.legal_entity).
                Include(p => p.agent).
                Include(p => p.insAmenities).
                Include(p => p.risk).
                Include(p => p.properties).ToListAsync();

			if (!User.IsInRole("Agent") && !User.IsInRole("Admin"))
			{
				contracts = contracts.Where(p => p.client.UserName == User.Identity.Name).ToList();
			}
			

			switch (type){
				case "0":
					contracts = contracts.Where(p => p.Is_problem).ToList();
					break;
			}

			switch (userType)
			{
				case "1":
					contracts = contracts.Where(p => p.client.legal_entity != null).ToList();
					break;
                case "2":
                    contracts = contracts.Where(p => p.client.legal_entity == null).ToList();
                    break;
            }

			switch (contrType)
			{
				case "1":
                    contracts = contracts.Where(p => p.insAmenities.Name == "Страхование жизни").ToList();
					break;
                case "2":
                    contracts = contracts.Where(p => p.insAmenities.Name != "Страхование жизни").ToList();
                    break;
            }

			if (startDate != null)
			{
				contracts = contracts.Where(p => DateTime.Compare(p.Start_date, startDate) >= 0).ToList();
			}

			if (endDate != null)
			{
				contracts = contracts.Where(p => DateTime.Compare(p.End_date, endDate) <= 0).ToList();
			}

			if(clientName != null)
			{
                contracts = contracts.Where(p => p.client.UserName == clientName).ToList();
            }

			if(agentName != null)
			{
                contracts = contracts.Where(p => p.agent.UserName == agentName).ToList();
            }

			return PartialView(contracts);
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
		public IActionResult Register()
		{
			return View();
		}

        [Authorize]
        [HttpGet]
		public IActionResult AddPaymReq()
		{
			return RedirectToAction("Create", "ReqforPaym");
		}

        [Authorize]
        public async Task<IActionResult> Details(int id)
		{
			Contract contract = await _Context.Contracts.Include(p => p.client)
				.Include(p => p.agent)
				.Include(p => p.insAmenities)
				.Include(p => p.risk)
				.Include(p => p.properties)
				.FirstOrDefaultAsync(p => p.num_of_contr == id);

			return View(contract);
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public async Task<IActionResult> Register(RegisterContractViewModel model)
		{
			if (ModelState.IsValid)
			{
                User client = await _userManager.FindByNameAsync(model.clientName);
                User agent = await _userManager.FindByNameAsync(User.Identity.Name);

				Risk risk = await _Context.riscs.FindAsync(model.riskId);

				InsAmenities insAmenities = await _Context.insAmenities.FindAsync(model.insAmenitId);

                model.errorText = "";
                model.errorRiskText = "";
                model.errorAmenitText = "";
                model.errorAmenitText = "";

                if (client == null)
				{
					model.errorText = "Пользователь не найден. Повторите ввод.";
					return View(model);
				}

                if (risk == null)
                {
                    model.errorRiskText = "Риск не найден. Повторите ввод.";
                    return View(model);
                }

                if (insAmenities == null)
                {
                    model.errorAmenitText = "Услуга страхования не найдена. Повторите ввод.";
                    return View(model);
                }

                client.UserName = client.Email;
                agent.UserName = agent.Email;

                Contract contract = new Contract
                {
                    Ins_premium = model.Ins_premium,
                    Ins_sum = model.Ins_sum,
                    arrears = model.arrears,
                    Start_date = model.Start_date,
                    End_date = model.End_date,
                    Is_problem = false
                };

                if (model.liveInsertion)
				{

					contract.client = client;
					contract.agent = agent;
					contract.insAmenities = insAmenities;
					contract.risk = risk;

                    var result = await _Context.AddAsync(contract);

                    await _Context.SaveChangesAsync();

					return RedirectToAction("Index");
				}
				else
				{

					AddPropertyViewModel.contract = contract;
                    AddPropertyViewModel.clientName = model.clientName;
                    AddPropertyViewModel.amenitId = model.insAmenitId;
                    AddPropertyViewModel.riskId = model.riskId;
                    AddPropertyViewModel.properties = new List<Property>();
                    AddPropertyViewModel.addingProperty = new List<Property>();
                    AddPropertyViewModel.indexes = new List<int>();


                    return View("AddProperty");
				}

			}

			return View(model);


		}


        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
		public IActionResult AddProperty(AddPropertyViewModel model)
		{
			return View(model);
		}

		[Authorize(Roles = "Agent, Admin")]
		[HttpGet]
		public IActionResult AddPropertyById(AddPropertyViewModel model)
		{
			AddPropByIdViewModel addPropByIdModel = new AddPropByIdViewModel();
			return View(addPropByIdModel);
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpGet]
		public IActionResult AddPropertyByData(AddPropertyViewModel model)
		{
			AddPropByDataViewModel addPropByDataModel = new AddPropByDataViewModel();

			return View(addPropByDataModel);
		}

        [Authorize]
        [HttpPost]
		public async Task<IActionResult> Delete(int id)
		{

			Contract contract = await _Context.Contracts.FindAsync(id);
			_Context.Contracts.Remove(contract);
			await _Context.SaveChangesAsync();

			return View("Index", _Context.Contracts.Include(p => p.client).Include(p => p.agent).ToList());

		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public IActionResult AddPropertyById(AddPropByIdViewModel addPropModel)
		{
			Property property = _Context.properties.Include(p => p.owner).FirstOrDefault(p => p.Id == addPropModel.Id);

			if (property == null)
			{
				addPropModel.errorText = "Имущество не найдено. Повторите ввод или выберите другой способ поиска.";
				return View(addPropModel);
			}
			AddPropertyViewModel.properties.Add(property);
            AddPropertyViewModel.indexes.Add(property.Id);

            return View("AddProperty");
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public async Task<IActionResult> AddPropertyByData(AddPropByDataViewModel addPropModel)
		{
			User owner = await _userManager.FindByNameAsync(addPropModel.ownerName);

            addPropModel.errorOwnText = "";
            addPropModel.errorText = "";

            if (owner == null)
			{
				addPropModel.errorOwnText = "Пользователь не найден, повторите ввод.";
				return View(addPropModel);
			}

			Property property = _Context.properties.FirstOrDefault(c => (c.Name == owner.UserName) && (c.descript == addPropModel.description) && (c.owner.Id == owner.Id));

            addPropModel.errorText = "";

			if (property == null)
			{
                if (!addPropModel.AddProperty)
                {
                    addPropModel.errorText = "Имущество не найденою Если вы хотите добавить это имущество в БД установите флажок 'Добавить в БД'";
                    return View(addPropModel);
                }

                property = new Property
				{
					Name = addPropModel.name,
					descript = addPropModel.description,
					owner = owner,
				};

                AddPropertyViewModel.addingProperty.Add(property);

                return View("AddProperty");
			}

			else
			{
				AddPropertyViewModel.properties.Add(property);
				AddPropertyViewModel.indexes.Add(property.Id);
				return View("AddProperty");
			}
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public async Task<IActionResult> RegisterPropertyContract()
		{
			Contract contract = AddPropertyViewModel.contract;

			contract.client = await _userManager.FindByNameAsync(AddPropertyViewModel.clientName);
            contract.agent = await _userManager.FindByNameAsync(User.Identity.Name);
			contract.insAmenities = await _Context.insAmenities.FindAsync(AddPropertyViewModel.amenitId);
			contract.risk = await _Context.riscs.FindAsync(AddPropertyViewModel.riskId);

			await _Context.AddAsync(contract);

			contract.properties = new List<Property>();

			foreach (var index in AddPropertyViewModel.indexes)
			{
				contract.properties.Add(await _Context.properties.FirstOrDefaultAsync(p => p.Id == index));
			}

			foreach (var property in AddPropertyViewModel.addingProperty)
			{
				property.owner = await _userManager.FindByIdAsync(property.owner.Id);
				contract.properties.Add(property);
			}

			await _Context.SaveChangesAsync();

            return RedirectToAction("Index");
		}

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public IActionResult CancelChange(int id)
		{
			Property property = AddPropertyViewModel.properties.FirstOrDefault(c => c.Id == id);
			AddPropertyViewModel.properties.Remove(property);
			return View("AddProperty");
        }

        [Authorize(Roles = "Agent, Admin")]
        [HttpPost]
		public IActionResult CancelAdding(int id)
		{
			Property property = AddPropertyViewModel.addingProperty.FirstOrDefault(c => c.Id == id);
			AddPropertyViewModel.addingProperty.Remove(property);
			return View("AddProperty");
		}
	}	
}
