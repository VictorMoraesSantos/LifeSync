---
title: ADR-004: CQRS + Result Pattern adoption across services
status: accepted
date: 2024-02-12
---

## Context
All services need consistent patterns for handling commands/queries and success/failure outcomes. Without consistent patterns:
- Error handling is inconsistent (some services throw exceptions, others return null)
- No clear separation between read and write operations
- Business logic validation and error reporting are scattered across handlers
- Testing error scenarios requires exception-based assertions

## Decision
Adopt CQRS (Separate Command and Query handlers via MediatR) and Result Pattern (Returning Result<T> instead of throwing exceptions). Each service uses:
- Commands (Write) processed by handlers that return `Result<T>`
- Queries (Read) processed by separate handlers
- Validation occurs in handlers, returning validation failures via Result type
- Domain events are dispatched through MediatR after command processing

## Consequences
### Positive
- Clear separation of read/write concerns following CQRS principles
- Composable queries - MediatR pipeline behaviors apply consistently
- Error handling via Result type enables functional error composition
- Validation failures are first-class return values, not exceptions
- Consistent pattern across all services simplifies onboarding

### Negative
- Learning curve for developers unfamiliar with Result pattern or CQRS
- More ceremony for simple CRUD operations
- Potential over-engineering for small, simple services
- MediatR adds a processing indirection layer

## Alternatives Considered
- Exception-based error handling: Rejected because exceptions should be for exceptional cases, not business rule violations
- Simple service methods returning DTOs: Rejected because it doesn't address the need for composable behaviors
- Only Result pattern without CQRS: Rejected because queries and commands have different optimization needs
- MediatR with exceptions for errors: Rejected because mixing exception handling with Result defeats the purpose of explicit error handling
