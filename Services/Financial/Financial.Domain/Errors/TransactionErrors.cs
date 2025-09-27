using BuildingBlocks.Results;

namespace Financial.Domain.Errors
{
    public static class TransactionErrors
    {
        // Erros de validação
        public static Error InvalidId => Error.Failure("O ID da transação deve ser maior que zero");
        public static Error InvalidUserId => Error.Failure("O ID do usuário deve ser maior que zero");
        public static Error InvalidCategoryId => Error.Failure("O ID da categoria deve ser maior que zero");
        public static Error InvalidAmount => Error.Failure("O valor da transação é obrigatório");
        public static Error InvalidDescription => Error.Failure("A descrição da transação é obrigatória");
        public static Error InvalidTransactionDate => Error.Failure("A data da transação é inválida");
        public static Error FutureTransactionDate => Error.Failure("A data da transação não pode ser no futuro");
        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Transação com ID {id} não encontrada");
        public static Error NotFound() => Error.NotFound($"Transações não encontradas");
        public static Error CreateError => Error.Problem("Erro ao criar transação");
        public static Error UpdateError => Error.Problem("Erro ao atualizar transação");
        public static Error DeleteError => Error.Problem("Erro ao excluir transação");
        // Erros de negócio
        public static Error CategoryNotFound => Error.NotFound("Categoria não encontrada");
        public static Error InvalidPaymentMethod => Error.Failure("Método de pagamento inválido");
        public static Error InvalidTransactionType => Error.Failure("Tipo de transação inválido");
        public static Error NegativeAmount => Error.Failure("O valor da transação deve ser positivo");
        public static Error RecurringTransactionError => Error.Problem("Erro ao processar transação recorrente");
    }
}