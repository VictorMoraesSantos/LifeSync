using BuildingBlocks.Results;

namespace TaskManager.Domain.Errors
{
    public static class TaskItemErrors
    {
        // Erros de validação
        public static Error InvalidTitle =>
            Error.Failure("TaskItem.InvalidTitle", "O título da tarefa é obrigatório");

        public static Error InvalidDescription =>
            Error.Failure("TaskItem.InvalidDescription", "A descrição da tarefa é obrigatória");

        public static Error DueDateInPast =>
            Error.Failure("TaskItem.DueDateInPast", "A data de vencimento não pode ser no passado");

        // Erros de labels
        public static Error NullLabel =>
            Error.Failure("TaskItem.NullLabel", "Label não pode ser nulo");

        public static Error DuplicateLabel =>
            Error.Failure("TaskItem.DuplicateLabel", "Label já existe para esta tarefa");

        public static Error LabelNotFound =>
            Error.Failure("TaskItem.LabelNotFound", "Label não encontrado nesta tarefa");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound("TaskItem.NotFound", $"Tarefa com ID {id} não encontrada");

        public static Error CreateError =>
            Error.Problem("TaskItem.CreateError", "Erro ao criar tarefa");

        public static Error UpdateError =>
            Error.Problem("TaskItem.UpdateError", "Erro ao atualizar tarefa");

        public static Error DeleteError =>
            Error.Problem("TaskItem.DeleteError", "Erro ao excluir tarefa");
    }
}