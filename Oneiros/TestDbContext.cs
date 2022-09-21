using Microsoft.EntityFrameworkCore;
using Oneiros.Auth;

namespace Oneiros;

public class TestDbContext : DbContext
{
    public DbSet<UserAccount> UserAccounts { get; set; }
    
    
}