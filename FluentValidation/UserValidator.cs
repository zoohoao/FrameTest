using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation
{
    internal class UserValidator : AbstractValidator<MainViewModel>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name)
           .NotEmpty().WithMessage("Name is required.")
           .Length(5, 50).WithMessage("Name must be between 2 and 50 characters.");
        }
    }
}