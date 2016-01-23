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
    public class StatusRequestValidator : AbstractValidator<StatusRequest>
    {
        public StatusRequestValidator()
        {
            RuleFor(s => s.GameId)
                .NotEqual(-1).WithMessage("Invalid game id, please use a valid game id")
                .Must(s => GameManager.Instance.GetAllActiveGames().Select(g => g.Id).Contains(s)).WithMessage("Invalid game id, please use a valid game id");
        }
    }
}
