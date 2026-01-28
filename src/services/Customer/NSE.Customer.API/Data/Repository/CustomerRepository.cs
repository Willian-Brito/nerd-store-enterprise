using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Customer.API.Data.Models.Interfaces;
using Entities = NSE.Customer.API.Models.Entities;

namespace NSE.Customer.API.Data.Repository.Customer;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerContext _context;
    public IUnitOfWork UnitOfWork => _context;
    
    public CustomerRepository(CustomerContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Entities.Customer>> GetAll()
    {
        return await _context.Customers.AsNoTracking().ToListAsync();
    }

    public Task<Entities.Customer> GetBySocialNumber(string ssn)
    {
        return _context.Customers.FirstOrDefaultAsync(c => c.SocialNumber == ssn);
    }

    public void Add(Entities.Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public async Task<Entities.Address> GetAddressById(Guid id)
    {
        return await _context.Addresses.FirstOrDefaultAsync(e => e.CustomerId == id);
    }

    public void AddAddress(Entities.Address address)
    {
        _context.Addresses.Add(address);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}