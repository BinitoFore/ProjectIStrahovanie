using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CurseProject.ViewModelsl
{
    public class EditPropViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название")]
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
