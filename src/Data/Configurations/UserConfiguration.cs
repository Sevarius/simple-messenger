using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property<long>("id")
            .UseIdentityColumn()
            .ValueGeneratedOnAdd()
            .HasColumnOrder(0);

        builder.Property(user => user.Id)
            .HasColumnName("user_id")
            .ValueGeneratedNever();

        builder.Property(user => user.UserName)
            .HasMaxLength(User.UserMaxLength)
            .IsRequired();

        builder.Property(user => user.IsDeleted);

        builder.HasKey(user => user.Id);

        builder.HasMany<Chat>()
            .WithMany(user => user.Users);
    }
}
