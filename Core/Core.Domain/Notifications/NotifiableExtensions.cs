using BuildingBlocks.Results;

namespace Core.Domain.Notifications
{
    public static class NotifiableExtensions
    {
        /// <summary>
        /// Converte os erros de validação em um array de strings com as descrições.
        /// </summary>
        public static string[] GetErrorMessages(this Notifiable notifiable)
        {
            return notifiable.Errors.Select(e => e.Description).ToArray();
        }

        /// <summary>
        /// Converte os erros de validação em uma única string concatenada.
        /// </summary>
        public static string GetErrorMessagesAsString(this Notifiable notifiable, string separator = "; ")
        {
            return string.Join(separator, notifiable.Errors.Select(e => e.Description).ToArray());
        }

        /// <summary>
        /// Converte as notificações em um Result com falha, mantendo os erros como array.
        /// </summary>
        public static Result<T> ToFailureResult<T>(this Notifiable notifiable)
        {
            if (notifiable.IsValid)
                throw new InvalidOperationException("Não é possível criar um Result de falha quando a entidade é válida.");

            var errorMessages = notifiable.GetErrorMessagesAsString();
            return Result.Failure<T>(new Error(errorMessages, ErrorType.Validation));
        }

        /// <summary>
        /// Obtém todos os tipos de erro.
        /// </summary>
        public static ErrorType[] GetErrorTypes(this Notifiable notifiable)
        {
            return notifiable.Errors.Select(e => e.Type).ToArray();
        }
    }
}