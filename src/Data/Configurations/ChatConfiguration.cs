using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("chats");

        builder.HasDiscriminator<string>("chat_type")
            .HasValue<PrivateChat>("private_chat")
            .HasValue<GroupChat>("group_chat");

        builder.Property<long>("id")
            .UseIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(0);

        builder.Property(chat => chat.Id)
            .HasColumnName("chat_id")
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(chat => chat.IdempotencyKey)
            .HasMaxLength(Chat.IdempotencyKeyMaxLength)
            .IsRequired();

        builder.Property(chat => chat.LastMessageTimestamp)
            .IsRequired();

        builder.Property(chat => chat.CreatedAt)
            .IsRequired();

        builder.Property<uint>("RowVersion")
            .IsRowVersion();

        builder.HasKey(chat => chat.Id);

        builder.HasIndex(chat => chat.IdempotencyKey)
            .IsUnique();

        builder.UseTphMappingStrategy();

        builder.HasMany<Message>()
            .WithOne()
            .HasForeignKey(message => message.ChatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(chat => chat.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
