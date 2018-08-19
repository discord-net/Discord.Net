using System.Collections.Generic;

namespace Discord.Commands
{
    /// <summary>
    ///     Utility methods for generating matching pairs of unicode quotation marks for CommandServiceConfig
    /// </summary>
    internal static class QuotationAliasUtils
    {
        /// <summary>
        ///     Generates an IEnumerable of characters representing open-close pairs of
        ///     quotation punctuation.
        /// </summary>
        internal static Dictionary<char, char> GetDefaultAliasMap => new Dictionary<char, char>
        {
            {'\"', '\"'},
            {'«', '»'},
            {'‘', '’'},
            {'“', '”'},
            {'„', '‟'},
            {'‹', '›'},
            {'‚', '‛'},
            {'《', '》'},
            {'〈', '〉'},
            {'「', '」'},
            {'『', '』'},
            {'〝', '〞'},
            {'﹁', '﹂'},
            {'﹃', '﹄'},
            {'＂', '＂'},
            {'＇', '＇'},
            {'｢', '｣'},
            {'(', ')'},
            {'༺', '༻'},
            {'༼', '༽'},
            {'᚛', '᚜'},
            {'⁅', '⁆'},
            {'⌈', '⌉'},
            {'⌊', '⌋'},
            {'❨', '❩'},
            {'❪', '❫'},
            {'❬', '❭'},
            {'❮', '❯'},
            {'❰', '❱'},
            {'❲', '❳'},
            {'❴', '❵'},
            {'⟅', '⟆'},
            {'⟦', '⟧'},
            {'⟨', '⟩'},
            {'⟪', '⟫'},
            {'⟬', '⟭'},
            {'⟮', '⟯'},
            {'⦃', '⦄'},
            {'⦅', '⦆'},
            {'⦇', '⦈'},
            {'⦉', '⦊'},
            {'⦋', '⦌'},
            {'⦍', '⦎'},
            {'⦏', '⦐'},
            {'⦑', '⦒'},
            {'⦓', '⦔'},
            {'⦕', '⦖'},
            {'⦗', '⦘'},
            {'⧘', '⧙'},
            {'⧚', '⧛'},
            {'⧼', '⧽'},
            {'⸂', '⸃'},
            {'⸄', '⸅'},
            {'⸉', '⸊'},
            {'⸌', '⸍'},
            {'⸜', '⸝'},
            {'⸠', '⸡'},
            {'⸢', '⸣'},
            {'⸤', '⸥'},
            {'⸦', '⸧'},
            {'⸨', '⸩'},
            {'【', '】'},
            {'〔', '〕'},
            {'〖', '〗'},
            {'〘', '〙'},
            {'〚', '〛'}
        };
    }
}
