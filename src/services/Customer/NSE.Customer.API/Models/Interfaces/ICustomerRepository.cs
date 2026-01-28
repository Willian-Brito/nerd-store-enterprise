using NSE.Core.Data;
using Entities = NSE.Customer.API.Models.Entities;

namespace NSE.Customer.API.Data.Models.Interfaces;

public interface ICustomerRepository : IRepository<Entities.Customer>
{
    void Add(Entities.Customer customer);

    Task<IEnumerable<Entities.Customer>> GetAll();
    Task<Entities.Customer> GetBySocialNumber(string ssn);

    void AddAddress(Entities.Address address);
    Task<Entities.Address> GetAddressById(Guid id);
}