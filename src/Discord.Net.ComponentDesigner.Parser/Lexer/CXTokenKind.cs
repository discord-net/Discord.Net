namespace Discord.CX.Parser;

public enum CXTokenKind : byte
{
    Invalid,
    EOF,

    LessThan,
    GreaterThan,
    ForwardSlashGreaterThan,
    LessThanForwardSlash,
    Equals,

    Text,
    Interpolation,

    StringLiteralStart,
    StringLiteralEnd,

    Identifier,
}
