using BuildingBlocks.Results;

namespace TaskManager.Domain.Errors
{
    public static class TaskLabelErrors
    {
        // Erros de validação
        public static Error InvalidName => Error.Failure("O nome do rótulo é obrigatório");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Rótulo com ID {id} não encontrado");
        public static Error CreateError => Error.Problem("Erro ao criar rótulo");
        public static Error UpdateError => Error.Problem("Erro ao atualizar rótulo");
        public static Error DeleteError => Error.Problem("Erro ao excluir rótulo");

        // Erros de listas
        public static Error EmptyOrNullList => Error.Failure("Lista de rótulos não pode ser nula ou vazia");
        public static Error InvalidIds => Error.Failure("Lista de IDs inválida ou vazia");
        public static Error SomeNotFound(IEnumerable<int> ids) => Error.NotFound($"Os seguintes rótulos não foram encontrados: {string.Join(", ", ids)}");
        public static Error AllNotFound => Error.NotFound("Nenhum dos rótulos foi encontrado");

        // Erros de consulta
        public static Error GetAllError => Error.Problem("Erro ao buscar rótulos");
        public static Error GetByIdError => Error.Problem("Erro ao buscar rótulo");
        public static Error FilterError => Error.Problem("Erro ao filtrar rótulos");
        public static Error FindError => Error.Problem("Erro ao buscar rótulos");
        public static Error CountError => Error.Problem("Erro ao contar rótulos");
        public static Error GetPagedError => Error.Problem("Erro ao obter página de rótulos");
        public static Error InvalidPagination => Error.Failure("Parâmetros de paginação inválidos");
    }
}