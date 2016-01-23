using FluentValidation;
using PlanetWars.Server;
using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Validators
{
    public class MoveRequestValidator : AbstractValidator<MoveRequest>
    {
        public MoveRequestValidator()
        {
            Guid trash;
            RuleFor(m => m.AuthToken)
                .NotEmpty().WithMessage("Auth token not present, move command failed")
                .Must(m => Guid.TryParse(m, out trash)).WithMessage("Auth token must be in guid format")
                .Must(m => GameManager.Instance.GetAllAuthTokens().Contains(m)).WithMessage("No logon exists with the supplied authentication code");
        }
    }
}
