using ECommerce.Application.DTOs;
using FluentValidation;

namespace ECommerce.Application.Validators
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ürün adı boş olamaz.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");
        }
    }
}