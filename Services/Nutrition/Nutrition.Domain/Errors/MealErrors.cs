using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class MealErrors
    {
        // Erros de validação
        public static Error InvalidId =>
            Error.Failure("Id deve ser um valor positivo");

        public static Error InvalidName =>
            Error.Failure("O nome da refeição é obrigatório");

        public static Error InvalidDescription =>
            Error.Failure("A descrição da refeição é obrigatória");

        public static Error InvalidDiaryId =>
            Error.Failure("DiaryId deve ser um valor positivo");

        // Erros de MealFood
        public static Error NullMealFood =>
            Error.Failure("MealFood não pode ser nulo");

        public static Error MealFoodNotFound =>
            Error.Failure("MealFood não encontrado nesta refeição");

        public static Error DuplicateMealFood =>
            Error.Failure("MealFood já existe nesta refeição");

        public static Error InvalidMealFoodId =>
            Error.Failure("ID do MealFood é inválido");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound($"Refeição com ID {id} não encontrada");

        public static Error NotFound() =>
            Error.NotFound($"Refeições não encontradas");

        public static Error CreateError =>
            Error.Problem("Erro ao criar refeição");

        public static Error UpdateError =>
            Error.Problem("Erro ao atualizar refeição");

        public static Error DeleteError =>
            Error.Problem("Erro ao excluir refeição");

        // Erros específicos do domínio
        public static Error NullValue(string parameterName) =>
            Error.Failure($"{parameterName} não pode ser nulo ou vazio");

        public static Error EmptyMealFoodList =>
            Error.Failure("A refeição deve conter pelo menos um alimento");

        public static Error InvalidCalories =>
            Error.Failure("O total de calorias deve ser um valor positivo");
    }
}