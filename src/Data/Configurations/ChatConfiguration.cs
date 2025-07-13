using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("chats");

        builder.Property<long>("id")
            .UseIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(0);

        builder.Property(chat => chat.Id)
            .HasColumnName("chat_id")
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasDiscriminator<string>("chat_type")
            .HasValue<PrivateChat>("private_chat")
            .HasValue<GroupChat>("group_chat");

        builder.HasKey(chat => chat.Id);

        builder.UseTphMappingStrategy();

        builder.HasMany<Message>()
            .WithOne()
            .HasForeignKey(chat => chat.ChatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
