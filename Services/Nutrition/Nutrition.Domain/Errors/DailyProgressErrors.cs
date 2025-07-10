using BuildingBlocks.Results;

namespace Nutrition.Domain.Errors
{
    public static class DailyProgressErrors
    {
        // Erros de validação
        public static Error InvalidUserId =>
            Error.Failure("UserId deve ser um valor válido e positivo");

        public static Error NegativeCalories =>
            Error.Failure("Calorias consumidas não podem ser negativas");

        public static Error NegativeLiquids =>
            Error.Failure("Quantidade de líquidos consumidos não pode ser negativa");

        public static Error NullGoal =>
            Error.Failure("Meta diária não pode ser nula");

        public static Error InvalidDate =>
            Error.Failure("Data deve ser válida");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound($"Progresso diário com ID {id} não encontrado");

        public static Error NotFound() =>
            Error.NotFound($"Progressos diários não encontrados");

        public static Error NotFoundByUserAndDate(int userId, DateOnly date) =>
            Error.NotFound($"Progresso diário para usuário {userId} na data {date:yyyy-MM-dd} não encontrado");

        public static Error CreateError =>
            Error.Problem("Erro ao criar progresso diário");

        public static Error UpdateError =>
            Error.Problem("Erro ao atualizar progresso diário");

        public static Error DeleteError =>
            Error.Problem("Erro ao excluir progresso diário");

        // Erros específicos do domínio
        public static Error NullDailyProgress =>
            Error.Failure("Progresso diário não pode ser nulo");

        public static Error DuplicateProgress =>
            Error.Failure("Já existe progresso registrado para este usuário nesta data");

        public static Error ProgressNotFound =>
            Error.Failure("Progresso diário não encontrado");

        // Erros de validação específicos
        public static Error InvalidCaloriesRange =>
            Error.Failure("Calorias consumidas devem estar dentro do intervalo válido");

        public static Error InvalidLiquidsRange =>
            Error.Failure("Quantidade de líquidos deve estar dentro do intervalo válido");

        public static Error ExcessiveCalories =>
            Error.Failure("Calorias consumidas excedem o limite permitido");

        public static Error ExcessiveLiquids =>
            Error.Failure("Quantidade de líquidos excede o limite permitido");

        // Erros de cálculo
        public static Error InvalidProgressCalculation =>
            Error.Failure("Erro no cálculo de progresso");

        public static Error InvalidPercentageCalculation =>
            Error.Failure("Erro no cálculo de porcentagem de progresso");

        public static Error GoalCalculationError =>
            Error.Failure("Erro no cálculo de meta");

        // Erros de parâmetros
        public static Error NullValue(string parameterName) =>
            Error.Failure($"{parameterName} não pode ser nulo ou vazio");

        public static Error InvalidParameter(string parameterName) =>
            Error.Failure($"Parâmetro {parameterName} é inválido");

        // Erros de associação
        public static Error UserNotFound =>
            Error.Failure("Usuário não encontrado");

        public static Error InvalidUserAssociation =>
            Error.Failure("Associação com usuário é inválida");

        // Erros de data
        public static Error FutureDate =>
            Error.Failure("Não é possível registrar progresso para datas futuras");

        public static Error InvalidDateRange =>
            Error.Failure("Intervalo de datas é inválido");

        public static Error DateTooOld =>
            Error.Failure("Data é muito antiga para registrar progresso");

        // Erros de meta
        public static Error GoalNotSet =>
            Error.Failure("Meta diária não foi definida");

        public static Error InvalidGoalValues =>
            Error.Failure("Valores da meta diária são inválidos");

        public static Error GoalAlreadyMet =>
            Error.Failure("Meta diária já foi atingida");

        // Erros de adição
        public static Error InvalidAdditionValue =>
            Error.Failure("Valor para adição deve ser positivo");

        public static Error AdditionOverflow =>
            Error.Failure("Adição resultaria em overflow de valores");
    }
}