---
title: ADR-002: Money Value Object for Multi-Currency (Financial Service)
status: accepted
date: 2024-01-22
---

## Context
Financial Service handles transactions in multiple currencies (BRL, USD, EUR, etc.). Simply using decimal for amounts is insufficient because:
- Adding BRL to USD without conversion produces incorrect results
- Currency context is lost when using raw decimals
- Invalid states like negative currency codes or negative amounts need validation

## Decision
Implement Money value object with amount (decimal) and currency (ISO 4217 string) as an immutable type. The Money type enforces:
- Positive amounts only (via constructor validation)
- Valid ISO 4217 currency codes
- Arithmetic operations that validate currency match before executing
- Explicit currency conversion methods for multi-currency operations

## Consequences
### Positive
- Type safety at the type system level prevents invalid operations
- Self-validating - invalid Money objects cannot be created
- Prevents currency mismatches (adding BRL to USD throws at runtime)
- Expressive domain model that matches real-world financial concepts

### Negative
- Requires conversion logic for multi-currency arithmetic operations
- Immutability requires creating new Money instances for adjustments
- Additional abstraction can make simple calculations more verbose

## Alternatives Considered
- Tuple (decimal amount, string currency): Rejected due to lack of validation and no type safety
- Primitive decimal with currency passed to each method: Rejected because currency context is easily lost
- Currency class without amount: Rejected because the pair is inseparable in financial calculations
- Third-party Money library: Rejected to maintain consistency with our existing domain model and avoid external dependencies
