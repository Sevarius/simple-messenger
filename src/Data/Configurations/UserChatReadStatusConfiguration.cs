using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public sealed class UserChatReadStatusConfiguration : IEntityTypeConfiguration<UserChatReadStatus>
{
    public void Configure(EntityTypeBuilder<UserChatReadStatus> builder)
    {
        builder.ToTable("user_chat_read_statuses");

        builder.Property<long>("id")
            .UseIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(0);

        builder.Property(userChatReadStatus => userChatReadStatus.Id)
            .HasColumnName("user_chat_read_status_id")
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(userChatReadStatus => userChatReadStatus.LastReadMessageTimestamp)
            .IsRequired();

        builder.Property<uint>("RowVersion")
            .IsRowVersion();

        builder.HasKey(userChatReadStatus => userChatReadStatus.Id);

        builder.HasIndex(userChatReadStatus => new { userChatReadStatus.ChatId, userChatReadStatus.UserId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(userChatReadStatus => userChatReadStatus.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Chat>()
            .WithMany(chat => chat.UserChatReadStatuses)
            .HasForeignKey(userChatReadStatus => userChatReadStatus.ChatId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
