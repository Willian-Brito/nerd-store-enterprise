using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NSE.Security.Identity.Data;

public class SecurityDbContext : IdentityDbContext
{ 
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options) { }
}