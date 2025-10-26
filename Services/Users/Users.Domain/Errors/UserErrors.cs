using BuildingBlocks.Results;

namespace Users.Domain.Errors
{
    public static class UserErrors
    {
        // Erros de valida��o
        public static Error NullName => Error.Failure("O nome � obrigat�rio");
        public static Error NullContact => Error.Failure("O contato � obrigat�rio");
        public static Error InvalidBirthDate => Error.Failure("A data de nascimento � inv�lida");
        public static Error BirthDateInFuture => Error.Failure("A data de nascimento n�o pode ser no futuro");

        // Conflitos e regras de neg�cio
        public static Error EmailAlreadyInUse(string email) => Error.Failure($"O email {email} j� est� em uso");
        public static Error UsernameAlreadyInUse(string username) => Error.Failure($"O nome de usu�rio {username} j� est� em uso");
        public static Error AlreadyActive => Error.Failure("O usu�rio j� est� ativo");
        public static Error AlreadyInactive => Error.Failure("O usu�rio j� est� inativo");
        public static Error AlreadyDeleted => Error.Failure("O usu�rio j� foi exclu�do");

        // Autentica��o/seguran�a
        public static Error InvalidCredentials => Error.Failure("Credenciais inv�lidas");
        public static Error RefreshTokenInvalid => Error.Failure("Refresh token inv�lido");
        public static Error RefreshTokenExpired => Error.Failure("Refresh token expirado");

        // Erros de opera��o
        public static Error NotFound(int id) => Error.NotFound($"Usu�rio com ID {id} n�o encontrado");
        public static Error CreateError => Error.Problem("Erro ao criar usu�rio");
        public static Error UpdateError => Error.Problem("Erro ao atualizar usu�rio");
        public static Error DeleteError => Error.Problem("Erro ao excluir usu�rio");
        public static Error ActivateError => Error.Problem("Erro ao ativar usu�rio");
        public static Error DeactivateError => Error.Problem("Erro ao desativar usu�rio");
        public static Error UpdateLastLoginError => Error.Problem("Erro ao atualizar �ltimo login do usu�rio");
    }
}