

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SchoolPilot.Data.Extensions
{
    public static class ModelBuilderExtension
    {
        public static EntityTypeBuilder<TEntity> AddIndex<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<EntityTypeBuilder<TEntity>> action) where TEntity : class
        {
            action(builder);
            return builder;
        }
    }
}
