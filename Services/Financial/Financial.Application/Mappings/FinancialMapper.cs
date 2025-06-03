using Financial.Application.DTOs.FinancialAccount;
using Financial.Domain.Entities;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Mappings
{
    public static class FinancialMapper
    {
        public static FinancialAccount ToEntity(CreateFinancialAccountDTO dto)
        {
            Money balance = Money.Create(dto.Balance.Amount, dto.Balance.Currency);
            FinancialAccount entity = new(dto.UserId, dto.Name, dto.AccountType, balance);
            return entity;
        }

        public static FinancialAccountDTO ToDTO(FinancialAccount entity)
        {
            FinancialAccountDTO dto = new(
                entity.Id,
                entity.UserId,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.AccountType,
                entity.Balance);
            return dto;
        }
    }
}
