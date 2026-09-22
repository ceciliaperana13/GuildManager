using GuildManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GuildManager.Infrastructure.Persistence;

public class GuildManagerDbContext : DbContext
{
    public GuildManagerDbContext(DbContextOptions<GuildManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<GuildInventory> GuildInventories => Set<GuildInventory>();
    public DbSet<CharacterTemplate> CharacterTemplates => Set<CharacterTemplate>();
    public DbSet<CharacterEquipment> CharacterEquipments => Set<CharacterEquipment>();
    public DbSet<GuildMember> GuildMembers => Set<GuildMember>();
    public DbSet<GuildMemberCharacter> GuildMemberCharacters => Set<GuildMemberCharacter>();
    public DbSet<Turn> Turns => Set<Turn>();
    public DbSet<RecruitmentOffer> RecruitmentOffers => Set<RecruitmentOffer>();
    public DbSet<QuestType> QuestTypes => Set<QuestType>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<QuestReward> QuestRewards => Set<QuestReward>();
    public DbSet<QuestMalus> QuestMaluses => Set<QuestMalus>();
    public DbSet<QuestAttempt> QuestAttempts => Set<QuestAttempt>();
    public DbSet<QuestAttemptMember> QuestAttemptMembers => Set<QuestAttemptMember>();
    public DbSet<QuestAttemptRewardLog> QuestAttemptRewardLogs => Set<QuestAttemptRewardLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Clés composites 
        modelBuilder.Entity<RecruitmentOffer>().HasKey(x => new { x.Id, x.GuildMemberId });
        modelBuilder.Entity<QuestReward>().HasKey(x => new { x.Id, x.QuestId });

        // Uniques
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<UserSession>().HasIndex(x => x.Token).IsUnique();

       
        modelBuilder.Entity<UserSession>()
            .HasOne<User>().WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Guild>()
            .HasOne<User>().WithMany()
            .HasForeignKey(x => x.FounderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GuildInventory>()
            .HasOne<Guild>().WithMany()
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GuildInventory>()
            .HasOne<Item>().WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CharacterTemplate>()
            .HasOne<CharacterClass>().WithMany()
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CharacterEquipment>()
            .HasOne<GuildMemberCharacter>().WithMany()
            .HasForeignKey(x => x.GuildMemberCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CharacterEquipment>()
            .HasOne<Item>().WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GuildMember>()
            .HasOne<User>().WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GuildMember>()
            .HasOne<Guild>().WithMany()
            .HasForeignKey(x => x.GuildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GuildMemberCharacter>()
            .HasOne<GuildMember>().WithMany()
            .HasForeignKey(x => x.GuildMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GuildMemberCharacter>()
            .HasOne<CharacterTemplate>().WithMany()
            .HasForeignKey(x => x.CharacterTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Turn>()
            .HasOne<GuildMember>().WithMany()
            .HasForeignKey(x => x.GuildMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecruitmentOffer>()
            .HasOne<GuildMember>().WithMany()
            .HasForeignKey(x => x.GuildMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecruitmentOffer>()
            .HasOne<CharacterTemplate>().WithMany()
            .HasForeignKey(x => x.CharacterTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Quest>()
            .HasOne<QuestType>().WithMany()
            .HasForeignKey(x => x.QuestTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestReward>()
            .HasOne<Quest>().WithMany()
            .HasForeignKey(x => x.QuestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestReward>()
            .HasOne<Item>().WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestMalus>()
            .HasOne<Quest>().WithMany()
            .HasForeignKey(x => x.QuestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestAttempt>()
            .HasOne<Quest>().WithMany()
            .HasForeignKey(x => x.QuestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestAttempt>()
            .HasOne<GuildMember>().WithMany()
            .HasForeignKey(x => x.GuildMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestAttemptMember>()
            .HasOne<QuestAttempt>().WithMany()
            .HasForeignKey(x => x.QuestAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestAttemptMember>()
            .HasOne<GuildMemberCharacter>().WithMany()
            .HasForeignKey(x => x.GuildMemberCharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuestAttemptRewardLog>()
            .HasOne<QuestAttempt>().WithMany()
            .HasForeignKey(x => x.QuestAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestAttemptRewardLog>()
            .HasOne<Item>().WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
