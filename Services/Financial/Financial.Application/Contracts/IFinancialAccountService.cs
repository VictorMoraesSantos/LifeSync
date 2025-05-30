using Core.Application.Interfaces;
using Financial.Application.DTOs.FinancialAccount;

namespace Financial.Application.Contracts
{
    public interface IFinancialAccountService
        : IReadService<FinancialAccountDTO, int>,
        ICreateService<CreateFinancialAccountDTO>,
        IUpdateService<UpdateFinancialAccountDTO>,
        IDeleteService<int>
    {
    }
}
