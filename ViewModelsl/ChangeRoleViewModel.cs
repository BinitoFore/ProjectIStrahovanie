using CurseProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CurseProject.ViewModelsl
{
	public class ChangeRoleViewModel
	{
		public List<SelectListItem> Roles { get; set; }	

		public User user { get; set; }

		public string roleName { get; set; }
	}
}
