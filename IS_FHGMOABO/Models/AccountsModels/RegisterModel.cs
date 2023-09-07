using System.ComponentModel.DataAnnotations;

namespace IS_FHGMOABO.Models.AccountsModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Требуется ввести имя пользователя")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Необходимо ввести пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Требуется ввести подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
