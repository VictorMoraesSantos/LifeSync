---
title: ADR-001: Strategy Pattern for Email Template Resolution (Notification Service)
status: accepted
date: 2024-01-15
---

## Context
Notification Service receives different types of integration events (UserRegistered, TaskDueReminder, etc.) and needs to send different email templates based on event type. Each event type requires a distinct email template, subject line, and potentially different placeholder replacements. Using a single switch/if-else chain to handle all event types would lead to a bloated service that violates the Open/Closed Principle.

## Decision
Use Strategy Pattern with IEmailEventStrategy interface. Each event type has its own strategy class (UserRegisteredEmailStrategy, TaskDueReminderEmailStrategy, etc.) that implements a common interface with a `SendEmailAsync` method. A StrategyResolver service maps event types to their corresponding strategy implementations.

## Consequences
### Positive
- Easy to add new email types without modifying existing code (Open/Closed Principle)
- Each strategy is independently testable
- Strategies can be injected and swapped at runtime
- Follows Single Responsibility Principle - each strategy handles one event type

### Negative
- More classes to maintain across the codebase
- Slight overhead for simple cases where a single handler would suffice
- Requires additional DI registration for each strategy

## Alternatives Considered
- Switch/if-else statement in a single EmailService: Rejected because it would require modifying the service for each new event type, violating OCP
- Dictionary mapping event types to delegates: Rejected due to limited testability and lack of type safety
- Base class inheritance hierarchy: Rejected because inheritance is less flexible than composition and creates tight coupling
