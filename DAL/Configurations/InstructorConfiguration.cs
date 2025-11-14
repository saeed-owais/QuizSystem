using Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuizSystem.DAL.Data.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(e => e.Id).HasDefaultValueSql("newsequentialid()");

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}