using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class MealFoodErrors
    {
        // Erros de validação
        public static Error InvalidId =>
            Error.Failure("O ID do alimento é obrigatório e deve ser um valor positivo");

        public static Error InvalidName =>
            Error.Failure("O nome do alimento é obrigatório");

        public static Error EmptyName =>
            Error.Failure("O nome do alimento não pode estar vazio");

        public static Error InvalidQuantity =>
            Error.Failure("A quantidade deve ser um valor positivo");

        public static Error NegativeCalories =>
            Error.Failure("As calorias por unidade não podem ser negativas");

        public static Error InvalidMealId =>
            Error.Failure("MealId deve ser um valor positivo");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound($"Alimento com ID {id} não encontrado");

        public static Error CreateError =>
            Error.Problem("Erro ao criar alimento");

        public static Error UpdateError =>
            Error.Problem("Erro ao atualizar alimento");

        public static Error DeleteError =>
            Error.Problem("Erro ao excluir alimento");

        // Erros específicos do domínio
        public static Error NullMealFood =>
            Error.Failure("Alimento não pode ser nulo");

        public static Error DuplicateMealFood =>
            Error.Failure("Alimento já existe na refeição");

        public static Error MealFoodNotFound =>
            Error.Failure("Alimento não encontrado na refeição");

        // Erros de validação específicos
        public static Error InvalidCaloriesCalculation =>
            Error.Failure("Erro no cálculo de calorias totais");

        public static Error ExcessiveQuantity =>
            Error.Failure("Quantidade de alimento excede o limite permitido");

        public static Error InvalidCaloriesRange =>
            Error.Failure("Calorias por unidade devem estar dentro do intervalo válido");

        public static Error ZeroCalories =>
            Error.Failure("Alimento deve conter pelo menos uma caloria por unidade");

        // Erros de parâmetros
        public static Error NullValue(string parameterName) =>
            Error.Failure($"{parameterName} não pode ser nulo ou vazio");

        // Erros de associação
        public static Error MealNotAssigned =>
            Error.Failure("Alimento deve estar associado a uma refeição");

        public static Error InvalidMealAssociation =>
            Error.Failure("Associação com refeição é inválida");
    }
}