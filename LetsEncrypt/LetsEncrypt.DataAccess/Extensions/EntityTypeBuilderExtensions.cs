using LetsEncrypt.Model.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsEncrypt.DataAccess.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void HasIdAndSysId<TModel>(this EntityTypeBuilder<TModel> entityBuilder) where TModel : ModelBase
    {
        entityBuilder.HasKey(setting => setting.Id).IsClustered(false);
        entityBuilder.Property(setting => setting.SysId).ValueGeneratedOnAdd();
        entityBuilder.HasIndex(setting => setting.SysId).IsClustered();
    }
}