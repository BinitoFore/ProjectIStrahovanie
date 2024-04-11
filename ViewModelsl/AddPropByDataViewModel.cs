using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
	public class AddPropByDataViewModel
	{
		[Required]
		[Display(Name = "Название")]
		public string name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Имя владельца")]
        public string ownerName { get; set; }

        [Display(Name = "Добавить имущество в БД")]
        public bool AddProperty { get; set; }

        public string errorText { get; set; } = "";

        public string errorOwnText { get; set; } = "";
	}
}
