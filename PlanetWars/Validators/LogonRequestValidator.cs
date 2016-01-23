using FluentValidation;
using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Validators
{
    public class LogonRequestValidator: AbstractValidator<LogonRequest>
    {
        public LogonRequestValidator()
        {
            RuleFor(logon => logon.AgentName)
                .NotEmpty().WithMessage("Please use a non empty agent name, logon failed")
                .Length(3, 15).WithMessage("Please use an agent name between 3-15 characters, logon failed");            
        }
    }
}
