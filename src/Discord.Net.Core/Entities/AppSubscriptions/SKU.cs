using System;

namespace Discord;

public struct SKU : ISnowflakeEntity
{
    /// <inheritdoc />
    public ulong Id { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <summary>
    ///     Gets the type of the SKU.
    /// </summary>
    public SKUType Type { get; }

    /// <summary>
    ///     Gets the ID of the parent application.
    /// </summary>
    public ulong ApplicationId { get; }

    /// <summary>
    ///     Gets the customer-facing name of your premium offering.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the system-generated URL slug based on the SKU's name.
    /// </summary>
    public string Slug { get; }

    /// <summary>
    ///     Gets the flags for this SKU.
    /// </summary>
    public SKUFlags Flags { get; }

    internal SKU(ulong id, SKUType type, ulong applicationId, string name, string slug, SKUFlags flags)
    {
        Id = id;
        Type = type;
        ApplicationId = applicationId;
        Name = name;
        Slug = slug;
        Flags = flags;
    }
}
