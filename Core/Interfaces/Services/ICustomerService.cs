using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer> CreateAsync(CreatePartnerRequest request, CancellationToken cancellationToken = default);
}

