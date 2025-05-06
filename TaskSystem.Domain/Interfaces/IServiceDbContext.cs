using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Interfaces;

public interface IServiceDbContext : IDisposable
{
    public DbSet<TaskTicket> TaskTickets { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public string? GetConnectionString();
    public Task<int> SaveChangesAsync();
    public T? Find<T>(Guid id) where T : Entity;
    public DbSet<T> Set<T>() where T : Entity;
    public Task<EntityEntry<T>> Add<T>(T entity) where T : Entity;
    public void Remove<T>(Guid id) where T : Entity;
    public void Update<T>(T entity) where T : Entity;
    public void UpdateRange<T>(T[] entity) where T : Entity;
    public void AttachRange<T>(T[] entities) where T : Entity;
    public EntityEntry<T> Attach<T>(T entity) where T : Entity;
    public void Detach<T>(T entity) where T : class;
    public EntityEntry<T> Entry<T>(T entity) where T : Entity;
    public void ClearTrackingObjects();
    public void AddRange<T>(List<T> entities) where T : Entity;
}