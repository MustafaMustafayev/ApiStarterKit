﻿using CORE.Abstract;
using DAL.EntityFramework.Seeds;
using ENTITIES.Entities;
using ENTITIES.Entities.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFramework.Context;

public class DataContext(
    DbContextOptions<DataContext> options,
    ITokenResolverService tokenResolverService)
    : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
    public required DbSet<RequestLog> RequestLogs { get; set; }
    public required DbSet<ResponseLog> ResponseLogs { get; set; }
    public required DbSet<Token> Tokens { get; set; }
    public required DbSet<Log> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditProperties();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /* migration commands
      dotnet ef --startup-project ../API migrations add initial --context DataContext
      dotnet ef --startup-project ../API database update --context DataContext
    */

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddGlobalFilter(nameof(Auditable.IsDeleted), false);

        modelBuilder.Entity<Token>().HasQueryFilter(m => !m.IsDeleted);

        DataSeed.Seed(modelBuilder);
    }

    private void SetAuditProperties()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: Auditable, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    ((Auditable)entityEntry.Entity).CreatedAt = DateTime.Now;
                    ((Auditable)entityEntry.Entity).CreatedById = tokenResolverService.GetUserIdFromToken();
                    break;
                case EntityState.Modified:
                    {
                        Entry((Auditable)entityEntry.Entity).Property(p => p.CreatedAt).IsModified =
                            false;
                        Entry((Auditable)entityEntry.Entity).Property(p => p.CreatedById).IsModified =
                            false;

                        if (((Auditable)entityEntry.Entity).IsDeleted)
                        {
                            Entry((Auditable)entityEntry.Entity).Property(p => p.ModifiedBy)
                                .IsModified = false;
                            Entry((Auditable)entityEntry.Entity).Property(p => p.ModifiedAt)
                                .IsModified = false;

                            ((Auditable)entityEntry.Entity).DeletedAt = DateTime.Now;
                            ((Auditable)entityEntry.Entity).DeletedBy = tokenResolverService.GetUserIdFromToken();
                        }
                        else
                        {
                            ((Auditable)entityEntry.Entity).ModifiedAt = DateTime.Now;
                            ((Auditable)entityEntry.Entity).ModifiedBy =
                                tokenResolverService.GetUserIdFromToken();
                        }

                        break;
                    }
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}