using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
    public class ReqForPaymModel
    {
        public int id { get; set; }

        [Required]
        [Display(Name ="Текст заявки")]
        public string description { get; set; }
    }
}
