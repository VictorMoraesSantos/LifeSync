using BuildingBlocks.Results;

namespace Gym.Domain.Errors
{
    internal static class RoutineExerciseErrors
    {
        // Erros de validação
        public static Error InvalidRoutineId => Error.Failure("ID da rotina é obrigatório e deve ser maior que zero");
        public static Error InvalidExerciseId => Error.Failure("ID do exercício é obrigatório e deve ser maior que zero");
        public static Error InvalidSets => Error.Failure("Quantidade de séries inválida");
        public static Error InvalidRepetitions => Error.Failure("Quantidade de repetições inválida");
        public static Error InvalidRestBetweenSets => Error.Failure("Tempo de descanso entre séries inválido");
        public static Error InvalidRecommendedWeight => Error.Failure("Peso recomendado inválido");
        public static Error InstructionsTooLong(int max) => Error.Failure($"Instruções devem ter no máximo {max} caracteres");

        // Relacionamentos / Consistência
        public static Error RoutineNotFound(int id) => Error.NotFound($"Rotina com ID {id} não encontrada");
        public static Error ExerciseNotFound(int id) => Error.NotFound($"Exercício com ID {id} não encontrado");
        public static Error ExerciseAlreadyInRoutine(int routineId, int exerciseId)
            => Error.Failure($"O exercício {exerciseId} já está associado à rotina {routineId}");
        public static Error ExerciseNotInRoutine(int routineId, int exerciseId)
            => Error.NotFound($"O exercício {exerciseId} não está associado à rotina {routineId}");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Exercício de rotina com ID {id} não encontrado");
        public static Error CreateError => Error.Problem("Erro ao criar exercício de rotina");
        public static Error UpdateError => Error.Problem("Erro ao atualizar exercício de rotina");
        public static Error DeleteError => Error.Problem("Erro ao excluir exercício de rotina");

        // Erros de listas
        public static Error EmptyOrNullList => Error.Failure("Lista de exercícios de rotina não pode ser nula ou vazia");
        public static Error InvalidIds => Error.Failure("Lista de IDs inválida ou vazia");
        public static Error SomeNotFound(IEnumerable<int> ids)
            => Error.NotFound($"Os seguintes exercícios de rotina não foram encontrados: {string.Join(", ", ids)}");
        public static Error AllNotFound => Error.NotFound("Nenhum dos exercícios de rotina foi encontrado");

        // Erros de consulta
        public static Error GetAllError => Error.Problem("Erro ao buscar exercícios de rotina");
        public static Error GetByIdError => Error.Problem("Erro ao buscar exercício de rotina");
        public static Error FilterError => Error.Problem("Erro ao filtrar exercícios de rotina");
        public static Error FindError => Error.Problem("Erro ao buscar exercícios de rotina");
        public static Error CountError => Error.Problem("Erro ao contar exercícios de rotina");
        public static Error GetPagedError => Error.Problem("Erro ao obter página de exercícios de rotina");
        public static Error InvalidPagination => Error.Failure("Parâmetros de paginação inválidos");
    }
}