using BuildingBlocks.Results;

namespace Financial.Domain.Errors
{
    public static class CategoryErrors
    {
        // Erros de validação
        public static Error InvalidId =>
            Error.Failure("O ID da categoria deve ser maior que zero");

        public static Error InvalidName =>
            Error.Failure("O nome da categoria é obrigatório");

        public static Error InvalidUserId =>
            Error.Failure("O ID do usuário deve ser maior que zero");

        // Erros de operação
        public static Error NotFound(int id) =>
            Error.NotFound($"Categoria com ID {id} não encontrada");

        public static Error NotFound() =>
            Error.NotFound($"Categorias não encontradas");

        public static Error CreateError =>
            Error.Problem("Erro ao criar categoria");

        public static Error UpdateError =>
            Error.Problem("Erro ao atualizar categoria");

        public static Error DeleteError =>
            Error.Problem("Erro ao excluir categoria");

        // Erros de negócio (caso você queira adicionar no futuro)
        public static Error DuplicateName =>
            Error.Conflict("Já existe uma categoria com este nome");

        public static Error CategoryInUse =>
            Error.Conflict("Não é possível excluir categoria que está sendo utilizada");
    }
}