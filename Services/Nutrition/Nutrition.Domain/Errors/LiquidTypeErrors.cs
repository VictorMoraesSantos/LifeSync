using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class LiquidTypeErrors
    {
        // Erros de validação
        public static Error InvalidName => Error.Failure("O nome do tipo de líquido é obrigatório");
        public static Error EmptyName => Error.Failure("O nome do tipo de líquido não pode estar vazio");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Tipo de líquido com ID {id} não encontrado");
        public static Error CreateError => Error.Problem("Erro ao criar tipo de líquido");
        public static Error UpdateError => Error.Problem("Erro ao atualizar tipo de líquido");
        public static Error DeleteError => Error.Problem("Erro ao excluir tipo de líquido");

        // Erros específicos do domínio
        public static Error NullLiquidType => Error.Failure("Tipo de líquido não pode ser nulo");
        public static Error DuplicateLiquidType => Error.Failure("Tipo de líquido já existe");

        // Erros de parâmetros
        public static Error NullValue(string parameterName) => Error.Failure($"{parameterName} não pode ser nulo ou vazio");
    }
}
