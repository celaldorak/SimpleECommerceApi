using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs
{
    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "TCKN zorunludur.")]
        public string TCKN { get; set; }


        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Ad ve soyad zorunludur.")]
        public string FullName { get; set; }
    }
}