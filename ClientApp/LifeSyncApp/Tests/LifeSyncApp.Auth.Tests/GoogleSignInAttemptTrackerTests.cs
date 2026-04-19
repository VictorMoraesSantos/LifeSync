using LifeSyncApp.Services.Auth;
using Xunit;

namespace LifeSyncApp.Auth.Tests;

public class GoogleSignInAttemptTrackerTests
{
    [Fact]
    public void BeginAttempt_StartsPendingExternalAuth()
    {
        var tracker = new GoogleSignInAttemptTracker();

        var attempt = tracker.BeginAttempt();

        Assert.Equal(GoogleSignInAttemptState.PendingExternalAuth, tracker.GetState(attempt.AttemptId));
    }

    [Fact]
    public void TryAcceptCallback_RejectsStaleCallbackState()
    {
        var tracker = new GoogleSignInAttemptTracker();
        var attempt = tracker.BeginAttempt();

        var accepted = tracker.TryAcceptCallback(attempt.AttemptId, "stale-state", out var error);

        Assert.False(accepted);
        Assert.Contains("stale", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryAcceptCallback_RejectsDuplicateCallbackForActiveAttempt()
    {
        var tracker = new GoogleSignInAttemptTracker();
        var attempt = tracker.BeginAttempt();

        var firstAccepted = tracker.TryAcceptCallback(attempt.AttemptId, attempt.AttemptId, out _);
        var secondAccepted = tracker.TryAcceptCallback(attempt.AttemptId, attempt.AttemptId, out var duplicateError);

        Assert.True(firstAccepted);
        Assert.False(secondAccepted);
        Assert.Contains("duplicado", duplicateError, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryMarkTimedOut_SetsTimedOutTerminalState()
    {
        var tracker = new GoogleSignInAttemptTracker();
        var attempt = tracker.BeginAttempt();

        tracker.TryMarkTimedOut(attempt.AttemptId, "timeout");

        Assert.Equal(GoogleSignInAttemptState.TimedOut, tracker.GetState(attempt.AttemptId));
    }

    [Fact]
    public void TryMarkCanceled_SetsCanceledTerminalState()
    {
        var tracker = new GoogleSignInAttemptTracker();
        var attempt = tracker.BeginAttempt();

        tracker.TryMarkCanceled(attempt.AttemptId, "canceled");

        Assert.Equal(GoogleSignInAttemptState.Canceled, tracker.GetState(attempt.AttemptId));
    }

    [Fact]
    public void SuccessPath_TransitionsToSucceeded()
    {
        var tracker = new GoogleSignInAttemptTracker();
        var attempt = tracker.BeginAttempt();

        var callbackAccepted = tracker.TryAcceptCallback(attempt.AttemptId, attempt.AttemptId, out _);
        var tokenExchangeMarked = tracker.TryMarkTokenExchange(attempt.AttemptId);
        tracker.TryMarkSucceeded(attempt.AttemptId);

        Assert.True(callbackAccepted);
        Assert.True(tokenExchangeMarked);
        Assert.Equal(GoogleSignInAttemptState.Succeeded, tracker.GetState(attempt.AttemptId));
    }
}
