using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Data.Configurations;

public sealed class PrivateChatConfiguration : IEntityTypeConfiguration<PrivateChat>
{
    public void Configure(EntityTypeBuilder<PrivateChat> builder)
    {
        // No additional configuration needed as it inherits from Chat
        // The base Chat configuration will handle the common properties
    }
} 