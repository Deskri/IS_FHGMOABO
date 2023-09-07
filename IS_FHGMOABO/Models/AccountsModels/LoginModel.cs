using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.AccountsModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Требуется ввести имя пользователя")]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
