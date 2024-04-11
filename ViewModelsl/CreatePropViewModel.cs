using CurseProject.Models;
using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
    public class CreatePropViewModel
    {
        [Required]
        [Display (Name = "Название")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Описание")]
        public string descript { get; set; }
        [Required]
        [Display(Name = "Имя аккаунта владельца")]
        public string ownerName { get; set; }

        public string errorText { get; set; } = "";
    }
}
