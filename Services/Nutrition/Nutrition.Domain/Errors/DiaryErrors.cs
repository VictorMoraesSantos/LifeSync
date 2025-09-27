using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class DiaryErrors
    {
        // Erros de validação
        public static Error InvalidId => Error.Failure("Id deve ser um valor positivo");
        public static Error InvalidUserId => Error.Failure("UserId deve ser um valor positivo");
        public static Error InvalidDate => Error.Failure("Data do diário é inválida");
        // Erros de refeições
        public static Error NullMeal => Error.Failure("Refeição não pode ser nula");
        public static Error MealNotFound => Error.Failure("Refeição não encontrada no diário");
        public static Error DuplicateMeal => Error.Failure("Refeição já existe no diário");
        // Erros de líquidos
        public static Error NullLiquid => Error.Failure("Líquido não pode ser nulo");
        public static Error LiquidNotFound => Error.Failure("Líquido não encontrado no diário");
        public static Error DuplicateLiquid => Error.Failure("Líquido já existe no diário");
        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Diário com ID {id} não encontrado");
        public static Error NotFound() => Error.NotFound($"Diários não encontrados");
        public static Error CreateError => Error.Problem("Erro ao criar diário");
        public static Error UpdateError => Error.Problem("Erro ao atualizar diário");
        public static Error DeleteError => Error.Problem("Erro ao excluir diário");
        // Erros específicos do domínio
        public static Error NullValue(string parameterName) => Error.Failure($"{parameterName} não pode ser nulo");
    }
}