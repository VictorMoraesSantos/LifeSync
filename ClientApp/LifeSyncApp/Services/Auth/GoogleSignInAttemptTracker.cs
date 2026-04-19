namespace LifeSyncApp.Services.Auth
{
    public enum GoogleSignInAttemptState
    {
        Idle = 0,
        PendingExternalAuth = 1,
        CallbackReceived = 2,
        TokenExchange = 3,
        Succeeded = 4,
        Failed = 5,
        TimedOut = 6,
        Canceled = 7
    }

    public sealed class GoogleSignInAttempt
    {
        public string AttemptId { get; }
        public GoogleSignInAttemptState State { get; private set; }
        public DateTime StartedAtUtc { get; }
        public DateTime? CompletedAtUtc { get; private set; }
        public string? FailureReason { get; private set; }

        internal GoogleSignInAttempt(string attemptId)
        {
            AttemptId = attemptId;
            State = GoogleSignInAttemptState.PendingExternalAuth;
            StartedAtUtc = DateTime.UtcNow;
        }

        internal bool IsTerminal => State is GoogleSignInAttemptState.Succeeded
            or GoogleSignInAttemptState.Failed
            or GoogleSignInAttemptState.TimedOut
            or GoogleSignInAttemptState.Canceled;

        internal void MoveTo(GoogleSignInAttemptState newState, string? reason = null)
        {
            State = newState;
            if (newState is GoogleSignInAttemptState.Succeeded
                or GoogleSignInAttemptState.Failed
                or GoogleSignInAttemptState.TimedOut
                or GoogleSignInAttemptState.Canceled)
            {
                CompletedAtUtc = DateTime.UtcNow;
                FailureReason = reason;
            }
        }
    }

    public sealed class GoogleSignInAttemptTracker
    {
        private readonly object _sync = new();
        private GoogleSignInAttempt? _activeAttempt;
        private readonly Dictionary<string, GoogleSignInAttemptState> _completedAttemptStates = new(StringComparer.Ordinal);

        public GoogleSignInAttempt BeginAttempt()
        {
            lock (_sync)
            {
                if (_activeAttempt is not null && !_activeAttempt.IsTerminal)
                {
                    throw new InvalidOperationException("Ja existe uma tentativa de login Google em andamento.");
                }

                var attempt = new GoogleSignInAttempt(Guid.NewGuid().ToString("N"));
                _activeAttempt = attempt;
                return attempt;
            }
        }

        public bool TryAcceptCallback(string attemptId, string? callbackState, out string error)
        {
            lock (_sync)
            {
                if (_activeAttempt is null || !_activeAttempt.AttemptId.Equals(attemptId, StringComparison.Ordinal))
                {
                    error = "Nenhuma tentativa ativa de login Google foi encontrada.";
                    return false;
                }

                if (_activeAttempt.State != GoogleSignInAttemptState.PendingExternalAuth)
                {
                    error = "Callback duplicado para a tentativa ativa foi ignorado.";
                    return false;
                }

                var effectiveCallbackState = string.IsNullOrWhiteSpace(callbackState)
                    ? attemptId
                    : callbackState;

                if (_completedAttemptStates.ContainsKey(effectiveCallbackState))
                {
                    error = "Callback duplicado de uma tentativa de login Google ja finalizada foi ignorado.";
                    return false;
                }

                if (!effectiveCallbackState.Equals(attemptId, StringComparison.Ordinal))
                {
                    error = "Callback stale de login Google foi ignorado por nao corresponder a tentativa ativa.";
                    return false;
                }

                _activeAttempt.MoveTo(GoogleSignInAttemptState.CallbackReceived);
                error = string.Empty;
                return true;
            }
        }

        public bool TryMarkTokenExchange(string attemptId)
        {
            lock (_sync)
            {
                if (!TryGetActiveAttempt(attemptId, out var attempt))
                {
                    return false;
                }

                if (attempt.State != GoogleSignInAttemptState.CallbackReceived)
                {
                    return false;
                }

                attempt.MoveTo(GoogleSignInAttemptState.TokenExchange);
                return true;
            }
        }

        public void TryMarkSucceeded(string attemptId)
        {
            TryMarkTerminal(attemptId, GoogleSignInAttemptState.Succeeded);
        }

        public void TryMarkFailed(string attemptId, string? reason)
        {
            TryMarkTerminal(attemptId, GoogleSignInAttemptState.Failed, reason);
        }

        public void TryMarkTimedOut(string attemptId, string? reason)
        {
            TryMarkTerminal(attemptId, GoogleSignInAttemptState.TimedOut, reason);
        }

        public void TryMarkCanceled(string attemptId, string? reason)
        {
            TryMarkTerminal(attemptId, GoogleSignInAttemptState.Canceled, reason);
        }

        public GoogleSignInAttemptState? GetState(string attemptId)
        {
            lock (_sync)
            {
                if (_activeAttempt is not null && _activeAttempt.AttemptId.Equals(attemptId, StringComparison.Ordinal))
                {
                    return _activeAttempt.State;
                }

                return _completedAttemptStates.TryGetValue(attemptId, out var state)
                    ? state
                    : null;
            }
        }

        private bool TryMarkTerminal(string attemptId, GoogleSignInAttemptState state, string? reason = null)
        {
            lock (_sync)
            {
                if (!TryGetActiveAttempt(attemptId, out var attempt) || attempt.IsTerminal)
                {
                    return false;
                }

                attempt.MoveTo(state, reason);
                _completedAttemptStates[attemptId] = state;
                _activeAttempt = null;
                return true;
            }
        }

        private bool TryGetActiveAttempt(string attemptId, out GoogleSignInAttempt attempt)
        {
            attempt = null!;

            if (_activeAttempt is null || !_activeAttempt.AttemptId.Equals(attemptId, StringComparison.Ordinal))
            {
                return false;
            }

            attempt = _activeAttempt;
            return true;
        }
    }
}
