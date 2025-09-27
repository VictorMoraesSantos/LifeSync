using BuildingBlocks.Results;

namespace Gym.Domain.Errors
{
    internal static class RoutineErrors
    {
        // Erros de validação
        public static Error InvalidName => Error.Failure("O nome da rotina é obrigatório");
        public static Error InvalidDescription => Error.Failure("A descrição da rotina é obrigatória");
        public static Error NameTooLong(int max) => Error.Failure($"O nome da rotina deve ter no máximo {max} caracteres");
        public static Error DescriptionTooLong(int max) => Error.Failure($"A descrição da rotina deve ter no máximo {max} caracteres");

        // Duplicidade
        public static Error DuplicateName(string name) => Error.Failure($"Já existe uma rotina com o nome \"{name}\"");

        // Relacionamentos / Consistência
        public static Error NullRoutineExercise => Error.Failure("Exercício da rotina não pode ser nulo");
        public static Error ExerciseAlreadyInRoutine(int routineId, int routineExerciseId)
            => Error.Failure($"O exercício de rotina {routineExerciseId} já está associado à rotina {routineId}");
        public static Error ExerciseNotFoundInRoutine(int routineId, int routineExerciseId)
            => Error.NotFound($"O exercício de rotina {routineExerciseId} não foi encontrado na rotina {routineId}");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Rotina com ID {id} não encontrada");
        public static Error CreateError => Error.Problem("Erro ao criar rotina");
        public static Error UpdateError => Error.Problem("Erro ao atualizar rotina");
        public static Error DeleteError => Error.Problem("Erro ao excluir rotina");
        public static Error AddExerciseError => Error.Problem("Erro ao adicionar exercício à rotina");
        public static Error RemoveExerciseError => Error.Problem("Erro ao remover exercício da rotina");

        // Erros de listas
        public static Error EmptyOrNullList => Error.Failure("Lista de rotinas não pode ser nula ou vazia");
        public static Error InvalidIds => Error.Failure("Lista de IDs inválida ou vazia");
        public static Error SomeNotFound(IEnumerable<int> ids)
            => Error.NotFound($"As seguintes rotinas não foram encontradas: {string.Join(", ", ids)}");
        public static Error AllNotFound => Error.NotFound("Nenhuma das rotinas foi encontrada");

        // Erros de consulta
        public static Error GetAllError => Error.Problem("Erro ao buscar rotinas");
        public static Error GetByIdError => Error.Problem("Erro ao buscar rotina");
        public static Error FilterError => Error.Problem("Erro ao filtrar rotinas");
        public static Error FindError => Error.Problem("Erro ao buscar rotinas");
        public static Error CountError => Error.Problem("Erro ao contar rotinas");
        public static Error GetPagedError => Error.Problem("Erro ao obter página de rotinas");
        public static Error InvalidPagination => Error.Failure("Parâmetros de paginação inválidos");
    }
}