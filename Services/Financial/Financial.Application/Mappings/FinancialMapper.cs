using Financial.Application.DTOs.FinancialAccount;
using Financial.Domain.Entities;

namespace Financial.Application.Mappings
{
    public static class FinancialMapper
    {
        public static FinancialAccount ToEntity(CreateFinancialAccountDTO dto)
        {
            FinancialAccount entity = new(dto.UserId, dto.Name, dto.AccountType, dto.Balance);
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
                entity.Balance,
                entity.Currency);
            return dto;
        }
    }
}
