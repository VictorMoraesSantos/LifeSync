using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class LiquidErrors
    {
        // Erros de validação
        public static Error InvalidName =>
            Error.Failure("O nome do líquido é obrigatório");

        public static Error EmptyName =>
            Error.Failure("O nome do líquido não pode estar vazio");

        public static Error InvalidQuantity =>
            Error.Failure("A quantidade deve ser um valor positivo");

        public static Error NegativeCalories =>
            Error.Failure("As calorias por ml não podem ser negativas");

        public static Error InvalidDiaryId =>
            Error.Failure("DiaryId deve ser um valor positivo");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound($"Líquido com ID {id} não encontrado");

        public static Error CreateError =>
            Error.Problem("Erro ao criar líquido");

        public static Error UpdateError =>
            Error.Problem("Erro ao atualizar líquido");

        public static Error DeleteError =>
            Error.Problem("Erro ao excluir líquido");

        // Erros específicos do domínio
        public static Error NullLiquid =>
            Error.Failure("Líquido não pode ser nulo");

        public static Error DuplicateLiquid =>
            Error.Failure("Líquido já existe no diário");

        public static Error LiquidNotFound =>
            Error.Failure("Líquido não encontrado no diário");

        // Erros de validação específicos
        public static Error InvalidCaloriesCalculation =>
            Error.Failure("Erro no cálculo de calorias totais");

        public static Error ExcessiveQuantity =>
            Error.Failure("Quantidade de líquido excede o limite permitido");

        public static Error InvalidCaloriesRange =>
            Error.Failure("Calorias por ml devem estar dentro do intervalo válido");

        // Erros de parâmetros
        public static Error NullValue(string parameterName) =>
            Error.Failure($"{parameterName} não pode ser nulo ou vazio");
    }
}