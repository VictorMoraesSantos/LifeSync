using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class FoodErrors
    {
        // Erros de validação
        public static Error InvalidId => Error.Failure("Id deve ser um valor positivo");
        public static Error InvalidName => Error.Failure("Nome do alimento é inválido");
        public static Error InvalidCalories => Error.Failure("Calorias devem ser um valor positivo");
        public static Error InvalidCarbohydrates => Error.Failure("Carboidratos devem ser um valor positivo");
        public static Error InvalidProteins => Error.Failure("Proteínas devem ser um valor positivo");
        public static Error InvalidFats => Error.Failure("Gorduras devem ser um valor positivo");
        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Alimento com ID {id} não encontrado");
        public static Error NotFound() => Error.NotFound($"Alimentos não encontrados");
        public static Error CreateError => Error.Problem("Erro ao criar alimento");
        public static Error UpdateError => Error.Problem("Erro ao atualizar alimento");
        public static Error DeleteError => Error.Problem("Erro ao excluir alimento");
    }
}
