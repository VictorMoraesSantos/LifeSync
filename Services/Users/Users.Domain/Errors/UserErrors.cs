using BuildingBlocks.Results;

namespace Users.Domain.Errors
{
    public static class UserErrors
    {
        // Erros de validação
        public static Error NullName => Error.Failure("O nome é obrigatório");
        public static Error NullContact => Error.Failure("O contato é obrigatório");
        public static Error InvalidBirthDate => Error.Failure("A data de nascimento é inválida");
        public static Error BirthDateInFuture => Error.Failure("A data de nascimento não pode ser no futuro");

        // Conflitos e regras de negócio
        public static Error EmailAlreadyInUse(string email) => Error.Failure($"O email {email} já está em uso");
        public static Error UsernameAlreadyInUse(string username) => Error.Failure($"O nome de usuário {username} já está em uso");
        public static Error AlreadyActive => Error.Failure("O usuário já está ativo");
        public static Error AlreadyInactive => Error.Failure("O usuário já está inativo");
        public static Error AlreadyDeleted => Error.Failure("O usuário já foi excluído");

        // Autenticação/segurança
        public static Error InvalidCredentials => Error.Failure("Credenciais inválidas");
        public static Error RefreshTokenInvalid => Error.Failure("Refresh token inválido");
        public static Error RefreshTokenExpired => Error.Failure("Refresh token expirado");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Usuário com ID {id} não encontrado");
        public static Error CreateError => Error.Problem("Erro ao criar usuário");
        public static Error UpdateError => Error.Problem("Erro ao atualizar usuário");
        public static Error DeleteError => Error.Problem("Erro ao excluir usuário");
        public static Error ActivateError => Error.Problem("Erro ao ativar usuário");
        public static Error DeactivateError => Error.Problem("Erro ao desativar usuário");
        public static Error UpdateLastLoginError => Error.Problem("Erro ao atualizar último login do usuário");
    }
}