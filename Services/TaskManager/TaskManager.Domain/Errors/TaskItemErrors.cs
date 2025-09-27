using BuildingBlocks.Results;

namespace TaskManager.Domain.Errors
{
    public static class TaskItemErrors
    {
        // Erros de validação
        public static Error InvalidTitle => Error.Failure("O título da tarefa é obrigatório");
        public static Error InvalidDescription => Error.Failure("A descrição da tarefa é obrigatória");
        public static Error DueDateInPast => Error.Failure("A data de vencimento não pode ser no passado");

        // Erros de labels
        public static Error NullLabel => Error.Failure("Label não pode ser nulo");
        public static Error DuplicateLabel => Error.Failure("Label já existe para esta tarefa");
        public static Error LabelNotFound => Error.Failure("Label não encontrado nesta tarefa");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Tarefa com ID {id} não encontrada");
        public static Error CreateError => Error.Problem("Erro ao criar tarefa");
        public static Error UpdateError => Error.Problem("Erro ao atualizar tarefa");
        public static Error DeleteError => Error.Problem("Erro ao excluir tarefa");
    }
}