using System;

namespace Discord.Interactions;

/// <summary>
///     Adds a file type to the marked field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalFileTypeAttribute : Attribute
{
    /// <summary>
    ///     The file type to add.
    /// </summary>
    public string FileType { get; }

    /// <summary>
    ///     Creates a new <see cref="ModalFileTypeAttribute"/>.
    /// </summary>
    /// <param name="fileType">The file type to add.</param>
    public ModalFileTypeAttribute(string fileType)
    {
        FileType = fileType;
    }
}
