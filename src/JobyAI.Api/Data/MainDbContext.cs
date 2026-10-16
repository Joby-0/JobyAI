using JobyAI.Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace JobyAI.Api.Data;

public sealed class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();
    public DbSet<Conversations> Conversations => Set<Conversations>();
    public DbSet<ChatMessageDB> ChatMessages => Set<ChatMessageDB>();
    public DbSet<ToolExecutions> ToolExecutions => Set<ToolExecutions>();
    public DbSet<PendingApprovals> PendingApprovals => Set<PendingApprovals>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(setting => setting.Id);
            entity.Property(setting => setting.Key).IsRequired();
            entity.Property(setting => setting.Value).IsRequired();
            entity.HasIndex(setting => setting.Key).IsUnique();
        });

        modelBuilder.Entity<Conversations>(entity =>
        {
            entity.HasKey(conversation => conversation.Id);
            entity.HasMany(conversation => conversation.Messages)
                .WithOne(message => message.Conversation)
                .HasForeignKey(message => message.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(conversation => conversation.ToolExecutions)
                .WithOne(execution => execution.Conversation)
                .HasForeignKey(execution => execution.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatMessageDB>(entity =>
        {
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Role).HasMaxLength(32);
        });

        modelBuilder.Entity<ToolExecutions>(entity =>
        {
            entity.HasKey(execution => execution.Id);
            entity.Property(execution => execution.Status).HasConversion<string>();
            entity.HasOne(execution => execution.PendingApproval)
                .WithOne(approval => approval.ToolExecution)
                .HasForeignKey<PendingApprovals>(approval => approval.ToolExecutionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PendingApprovals>(entity =>
        {
            entity.HasKey(approval => approval.Id);
            entity.Property(approval => approval.Status).HasConversion<string>();
        });
    }
}
