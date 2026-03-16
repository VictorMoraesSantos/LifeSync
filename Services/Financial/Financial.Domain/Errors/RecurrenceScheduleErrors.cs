using BuildingBlocks.Results;

namespace Financial.Domain.Errors
{
    public static class RecurrenceScheduleErrors
    {
        // Erros de validação
        public static Error InvalidId => Error.Failure("O ID do agendamento deve ser maior que zero");
        public static Error InvalidFrequency => Error.Failure("A frequência de recorrência é inválida");
        public static Error InvalidStartDate => Error.Failure("A data de início é obrigatória");
        public static Error EndDateBeforeStartDate => Error.Failure("A data final deve ser posterior à data de início");
        public static Error InvalidMaxOccurrences => Error.Failure("O número máximo de ocorrências deve ser maior que zero");
        public static Error InvalidTransaction => Error.Failure("A transação de origem é obrigatória");

        // Erros de operação
        public static Error NotFound(int id) => Error.NotFound($"Agendamento com ID {id} não encontrado");
        public static Error NotFound() => Error.NotFound("Agendamentos não encontrados");
        public static Error CreateError => Error.Problem("Erro ao criar agendamento de recorrência");
        public static Error UpdateError => Error.Problem("Erro ao atualizar agendamento de recorrência");
        public static Error DeleteError => Error.Problem("Erro ao excluir agendamento de recorrência");

        // Erros de negócio
        public static Error InactiveSchedule => Error.Failure("Este agendamento está inativo");
        public static Error MaxOccurrencesReached => Error.Failure("Número máximo de ocorrências atingido");
        public static Error TransactionNotLoaded => Error.Failure("A transação de origem não foi carregada");
        public static Error TransactionNotRecurring => Error.Failure("A transação de origem não está marcada como recorrente");
        public static Error ScheduleAlreadyExists => Error.Failure("Já existe um agendamento para esta transação");
        public static Error CannotReactivate => Error.Failure("Não é possível reativar: o agendamento já atingiu o limite de ocorrências ou ultrapassou a data final");
        public static Error ProcessingError => Error.Problem("Erro ao processar transações recorrentes");
    }
}
