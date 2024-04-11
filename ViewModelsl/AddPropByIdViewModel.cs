using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
	public class AddPropByIdViewModel: AddPropertyViewModel
	{
		[Required]
		[Display(Name = "Id")]
		public int Id { get; set; }

		public string errorText { get; set; } = "";
	}
}
