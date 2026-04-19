---
title: ADR-003: RabbitMQ for Service Integration
status: accepted
date: 2024-02-05
---

## Context
Services need to communicate (e.g., Users Service needs to notify Notification Service when a user registers). Direct HTTP coupling would create tight coupling between services, making them dependent on each other's availability. This creates resilience issues and prevents independent deployment and scaling.

## Decision
Use RabbitMQ as an event bus. Publishers send to exchanges with routing keys, consumers subscribe to queues. Services communicate through events rather than direct calls:
- Publishers declare events with routing keys (e.g., `user.registered`, `task.due`)
- Consumers subscribe to queues bound to relevant exchanges
- Message broker handles delivery, retry, and dead-lettering

## Consequences
### Positive
- Fully decoupled services - publishers and consumers have no compile-time or deployment dependency
- Async processing - services can handle load spikes without synchronous bottlenecks
- Built-in retry mechanisms with dead letter queues for failed messages
- Event persistence allows replay of missed events
- Services can scale independently

### Negative
- Added infrastructure complexity (RabbitMQ server, exchanges, queues)
- Eventual consistency - consumers may process events seconds after publication
- Message ordering across consumers can be challenging
- Debugging distributed systems is more complex
- Requires careful message schema versioning

## Alternatives Considered
- Direct HTTP calls: Rejected due to tight coupling and synchronous availability requirements
- REST with message queue behind service: Rejected because the service would still need to manage queue consumers
- gRPC streaming: Rejected because it maintains tighter coupling than message-based approaches
- Azure Service Bus: Rejected in favor of RabbitMQ due to team familiarity and self-hosting requirements
