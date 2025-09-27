using BuildingBlocks.Results;

namespace Gym.Domain.Errors
{
    internal static class TrainingSessionErrors
    {
        // Erros de validação
        public static Error InvalidUserId => Error.Failure("ID do usuário é obrigatório e deve ser maior que zero");
        public static Error InvalidRoutineId => Error.Failure("ID da rotina é obrigatório e deve ser maior que zero");
        public static Error InvalidStartTime => Error.Failure("Hora de início inválida");
        public static Error InvalidEndTime => Error.Failure("Hora de término inválida");
        public static Error EndTimeNotAfterStart => Error.Failure("Hora de término deve ser posterior à hora de início");
        public static Error NotesTooLong(int max) => Error.Failure($"Observações devem ter no máximo {max} caracteres");

        // Relacionamentos / Consistência
        public static Error UserNotFound(int id) => Error.NotFound($"Usuário com ID {id} não encontrado");
        public static Error RoutineNotFound(int id) => Error.NotFound($"Rotina com ID {id} não encontrada");
        public static Error CompletedExerciseNull => Error.Failure("Exercício concluído não pode ser nulo");
        public static Error CompletedExerciseFromAnotherSession(int completedExerciseId, int sessionId, int otherSessionId)
            => Error.Failure($"O exercício concluído {completedExerciseId} pertence à sessão {otherSessionId}, não à sessão {sessionId}");
        public static Error CompletedExerciseAlreadyInSessionByRoutineExercise(int routineExerciseId)
            => Error.Failure($"Já existe um exercício concluído para o exercício de rotina {routineExerciseId} nesta sessão");
        public static Error CompletedExerciseNotFoundInSession(int completedExerciseId, int sessionId)
            => Error.NotFound($"Exercício concluído {completedExerciseId} não encontrado na sessão {sessionId}");

        // Regras de estado
        public static Error SessionAlreadyCompleted => Error.Failure("A sessão de treino já foi concluída");
        public static Error SessionNotCompleted => Error.Failure("A sessão de treino ainda não foi concluída");
        public static Error CannotAddExerciseToCompletedSession => Error.Failure("Não é possível adicionar exercícios a uma sessão já concluída");
        public static Error CannotUpdateCompletedSession => Error.Failure("Não é possível atualizar uma sessão já concluída");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Sessão de treino com ID {id} não encontrada");
        public static Error CreateError => Error.Problem("Erro ao criar sessão de treino");
        public static Error UpdateError => Error.Problem("Erro ao atualizar sessão de treino");
        public static Error DeleteError => Error.Problem("Erro ao excluir sessão de treino");
        public static Error CompleteError => Error.Problem("Erro ao concluir sessão de treino");
        public static Error AddCompletedExerciseError => Error.Problem("Erro ao adicionar exercício concluído à sessão de treino");
        public static Error RemoveCompletedExerciseError => Error.Problem("Erro ao remover exercício concluído da sessão de treino");

        // Erros de listas
        public static Error EmptyOrNullList => Error.Failure("Lista de sessões de treino não pode ser nula ou vazia");
        public static Error InvalidIds => Error.Failure("Lista de IDs inválida ou vazia");
        public static Error SomeNotFound(IEnumerable<int> ids)
            => Error.NotFound($"As seguintes sessões de treino não foram encontradas: {string.Join(", ", ids)}");
        public static Error AllNotFound => Error.NotFound("Nenhuma das sessões de treino foi encontrada");

        // Erros de consulta
        public static Error GetAllError => Error.Problem("Erro ao buscar sessões de treino");
        public static Error GetByIdError => Error.Problem("Erro ao buscar sessão de treino");
        public static Error FilterError => Error.Problem("Erro ao filtrar sessões de treino");
        public static Error FindError => Error.Problem("Erro ao buscar sessões de treino");
        public static Error CountError => Error.Problem("Erro ao contar sessões de treino");
        public static Error GetPagedError => Error.Problem("Erro ao obter página de sessões de treino");
        public static Error InvalidPagination => Error.Failure("Parâmetros de paginação inválidos");
    }
}