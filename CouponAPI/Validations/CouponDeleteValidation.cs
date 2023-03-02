using CouponAPI.Models.Dto;
using FluentValidation;

namespace CouponAPI.Validations
{
    public class CouponDeleteValidation : AbstractValidator<CouponDeleteDTO>
    {
        public CouponDeleteValidation() {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
        }
    }
}
