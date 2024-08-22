using ChatManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatManagement.DataAccess.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasData(new List<Chat>
        {
            new Chat
            {
                Id = Guid.NewGuid(),
                Title = "Chat 1",
                CreatedAt = DateTime.Now,
                UserIds = new List<Guid> { Guid.NewGuid() }
            },
            new Chat
            {
                Id = Guid.NewGuid(),
                Title = "Chat 2",
                CreatedAt = DateTime.Now,
                UserIds = new List<Guid> { Guid.NewGuid() }
            }
        });
    }
}