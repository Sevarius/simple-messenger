using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Data;

public sealed class MessengerDbContext : DbContext
{
    public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PrivateChat> PrivateChats { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
