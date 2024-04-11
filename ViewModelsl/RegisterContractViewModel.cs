using CurseProject.Models;
using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
    public class RegisterContractViewModel
    {
        [Required]
        [Display(Name = "Имя клиента")]
        public string clientName { get; set; }

        [Required]
        [Display (Name = "Id услуги страхования")]
        public int insAmenitId { get; set; }
        [Required]
        [Display(Name = "Id риска")]
        public int riskId { get; set; }
        [Required]
        [Display(Name = "Страховая премия")]
        public float Ins_premium { get; set; }

        [Required]
        [Display(Name = "Страховая сумма")]
        public float Ins_sum { get; set; }

        [Required]
        [Display(Name = "Задолженность")]
        public float arrears { get; set; }

        [Required]
        [Display(Name = "Дата вступления в силу")]
        public DateTime Start_date { get; set; }

        [Required]
        [Display(Name = "Дата окончания действия")]
        public DateTime End_date { get; set; }

        [Required]
        [Display(Name = "Договор страхования жизни")]
        public bool liveInsertion { get; set; }

        public string errorText { get; set; } = "";
        public string errorAmenitText { get; set; } = "";
        public string errorRiskText { get; set; } = "";
    }
}
