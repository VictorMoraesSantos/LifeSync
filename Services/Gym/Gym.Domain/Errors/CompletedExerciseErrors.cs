using BuildingBlocks.Results;

namespace Gym.Domain.Errors
{
    public static class CompletedExerciseErrors
    {
        // Erros de validação
        public static Error InvalidTrainingSessionId => Error.Failure("ID da sessão de treino é obrigatório e deve ser maior que zero");
        public static Error InvalidRoutineExerciseId => Error.Failure("ID do exercício da rotina é obrigatório e deve ser maior que zero");
        public static Error InvalidSetsCompleted => Error.Failure("Quantidade de séries concluídas inválida");
        public static Error InvalidRepetitionsCompleted => Error.Failure("Quantidade de repetições concluídas inválida");
        public static Error InvalidWeight => Error.Failure("Peso utilizado inválido");
        public static Error CompletionDateInFuture => Error.Failure("A data de conclusão não pode estar no futuro");
        public static Error NotesTooLong(int max) => Error.Failure($"Observações devem ter no máximo {max} caracteres");

        // Erros de relacionamento
        public static Error TrainingSessionNotFound(int id) => Error.NotFound($"Sessão de treino com ID {id} não encontrada");
        public static Error RoutineExerciseNotFound(int id) => Error.NotFound($"Exercício de rotina com ID {id} não encontrado");
        public static Error RoutineExerciseNotInSession(int routineExerciseId, int trainingSessionId)
            => Error.Failure($"O exercício de rotina {routineExerciseId} não pertence à sessão de treino {trainingSessionId}");

        // Duplicidade
        public static Error AlreadyRegisteredInSession(int trainingSessionId, int routineExerciseId)
            => Error.Failure($"Já existe um exercício concluído para o exercício de rotina {routineExerciseId} na sessão {trainingSessionId}");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Exercício concluído com ID {id} não encontrado");
        public static Error CreateError => Error.Problem("Erro ao registrar exercício concluído");
        public static Error UpdateError => Error.Problem("Erro ao atualizar exercício concluído");
        public static Error DeleteError => Error.Problem("Erro ao excluir exercício concluído");

        // Erros de listas
        public static Error EmptyOrNullList => Error.Failure("Lista de exercícios concluídos não pode ser nula ou vazia");
        public static Error InvalidIds => Error.Failure("Lista de IDs inválida ou vazia");
        public static Error SomeNotFound(IEnumerable<int> ids) => Error.NotFound($"Os seguintes exercícios concluídos não foram encontrados: {string.Join(", ", ids)}");
        public static Error AllNotFound => Error.NotFound("Nenhum dos exercícios concluídos foi encontrado");

        // Erros de consulta
        public static Error GetAllError => Error.Problem("Erro ao buscar exercícios concluídos");
        public static Error GetByIdError => Error.Problem("Erro ao buscar exercício concluído");
        public static Error FilterError => Error.Problem("Erro ao filtrar exercícios concluídos");
        public static Error FindError => Error.Problem("Erro ao buscar exercícios concluídos");
        public static Error CountError => Error.Problem("Erro ao contar exercícios concluídos");
        public static Error GetPagedError => Error.Problem("Erro ao obter página de exercícios concluídos");
        public static Error InvalidPagination => Error.Failure("Parâmetros de paginação inválidos");
    }
}