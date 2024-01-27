namespace Host.Db.EntityConfigurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .Property(x => x.Description)
            .HasMaxLength(256);

        builder
            .ToTable("Roles");
    }
}
