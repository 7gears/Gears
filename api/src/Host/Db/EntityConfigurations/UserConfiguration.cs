namespace Host.Db.EntityConfigurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(x => x.FirstName)
            .HasMaxLength(256);

        builder
            .Property(x => x.LastName)
            .HasMaxLength(256);

        builder.ToTable("Users");
    }
}
