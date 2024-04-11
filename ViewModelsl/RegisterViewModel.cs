using System.ComponentModel.DataAnnotations;

namespace CurseProject.ViewModelsl
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "SecName")]
        public string SecName { get; set; }

        [Required]
        [Display(Name = "Pathonymic")]
        public string Pathonymic { get; set; }

        [Required]
        [MaxLength(11)]
        [MinLength(11)]
        [Display(Name = "TelNumber")]
        public string TelNumber { get; set; }

        [Required]
        [MaxLength(10)]
        [MinLength(10)]
        [Display(Name = "PassNumber")]
        public string PassNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match!")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Код администратора")]
        public string? code { get; set; }

        [Required]
        [Display(Name = "Зарегистрироваться как юридическое лицо. Без установки галочки поля ниже будут проинорированы.")]
        public bool legalEntity { get; set; }

        [Display(Name = "Юридический адрес")]
        public string? Leg_adress { get; set; }

        [Display(Name = "Расчетный счет")]
        public string? Paym_account { get; set; }

        [Display(Name = "Название организации")]
        public string? Org_name { get; set; }

    }
}
