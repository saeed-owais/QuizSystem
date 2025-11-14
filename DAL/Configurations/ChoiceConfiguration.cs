using Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuizSystem.DAL.Data.Configurations
{
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("newsequentialid()");

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}