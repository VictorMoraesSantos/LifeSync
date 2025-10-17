using BuildingBlocks.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Errors
{
    public static class NameErrors
    {
        public static Error NullName => Error.Failure("O nome não pode ser nulo ou vazio");
    }
}
