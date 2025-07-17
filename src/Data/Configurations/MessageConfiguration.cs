using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.Property<long>("id")
            .UseIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(0);

        builder.Property(message => message.Id)
            .HasColumnName("message_id")
            .ValueGeneratedNever();

        builder.Property(message => message.ChatId)
            .IsRequired();

        builder.Property(message => message.UserId)
            .IsRequired();

        builder.Property(message => message.Content)
            .HasMaxLength(Message.MessageMaxLength)
            .IsRequired();

        builder.Property(message => message.IsModified)
            .HasDefaultValue(false);

        builder.Property(message => message.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(message => message.CreatedAt)
            .IsRequired();

        builder.Property<uint>("RowVersion")
            .IsRowVersion();

        builder.HasIndex(message => message.ChatId);
        builder.HasIndex(message => message.UserId);
        builder.HasIndex(message => message.CreatedAt);

        builder.HasKey(message => message.Id);
    }
}
