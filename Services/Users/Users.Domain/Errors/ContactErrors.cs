using BuildingBlocks.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Errors
{
    public static class ContactErrors
    {
        public static Error InvalidFormat => Error.Failure("O formato do email não é válido");
    }
}
