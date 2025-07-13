using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class GroupChatConfiguration : IEntityTypeConfiguration<GroupChat>
{
        public void Configure(EntityTypeBuilder<GroupChat> builder)
    {
        builder.Property(groupChat => groupChat.CreatorId)
            .IsRequired();

        builder.Property(groupChat => groupChat.Name)
            .HasMaxLength(GroupChat.GroupChatMaxLength)
            .IsRequired();

        builder.HasIndex(groupChat => groupChat.CreatorId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(groupChat => groupChat.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
