using BuildingBlocks.Results;

namespace TaskManager.Domain.Errors
{
    public static class TaskLabelErrors
    {
        // Erros de validação
        public static Error InvalidName =>
            Error.Failure("TaskLabel.InvalidName", "O nome do rótulo é obrigatório");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound("TaskLabel.NotFound", $"Rótulo com ID {id} não encontrado");

        public static Error CreateError =>
            Error.Problem("TaskLabel.CreateError", "Erro ao criar rótulo");

        public static Error UpdateError =>
            Error.Problem("TaskLabel.UpdateError", "Erro ao atualizar rótulo");

        public static Error DeleteError =>
            Error.Problem("TaskLabel.DeleteError", "Erro ao excluir rótulo");

        // Erros de listas
        public static Error EmptyOrNullList =>
            Error.Failure("TaskLabel.EmptyOrNullList", "Lista de rótulos não pode ser nula ou vazia");

        public static Error InvalidIds =>
            Error.Failure("TaskLabel.InvalidIds", "Lista de IDs inválida ou vazia");

        public static Error SomeNotFound(IEnumerable<int> ids) =>
            Error.NotFound("TaskLabel.SomeNotFound", $"Os seguintes rótulos não foram encontrados: {string.Join(", ", ids)}");

        public static Error AllNotFound =>
            Error.NotFound("TaskLabel.AllNotFound", "Nenhum dos rótulos foi encontrado");

        // Erros de consulta
        public static Error GetAllError =>
            Error.Problem("TaskLabel.GetAllError", "Erro ao buscar rótulos");

        public static Error GetByIdError =>
            Error.Problem("TaskLabel.GetByIdError", "Erro ao buscar rótulo");

        public static Error FilterError =>
            Error.Problem("TaskLabel.FilterError", "Erro ao filtrar rótulos");

        public static Error FindError =>
            Error.Problem("TaskLabel.FindError", "Erro ao buscar rótulos");

        public static Error CountError =>
            Error.Problem("TaskLabel.CountError", "Erro ao contar rótulos");

        public static Error GetPagedError =>
            Error.Problem("TaskLabel.GetPagedError", "Erro ao obter página de rótulos");

        public static Error InvalidPagination =>
            Error.Failure("TaskLabel.InvalidPagination", "Parâmetros de paginação inválidos");
    }
}