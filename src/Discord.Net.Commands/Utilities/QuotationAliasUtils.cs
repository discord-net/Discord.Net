using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Discord.Commands
{
    /// <summary>
    /// Utility methods for generating matching pairs of unicode quotation marks for CommandServiceConfig
    /// </summary>
    internal static class QuotationAliasUtils
    {
        /// <summary>
        /// Generates an IEnumerable of characters representing open-close pairs of
        /// quotation punctuation.
        /// </summary>
        internal static Dictionary<char, char> GetDefaultAliasMap
        {
            get
            {
                // Output of a gist provided by https://gist.github.com/ufcpp
                // https://gist.github.com/ufcpp/5b2cf9a9bf7d0b8743714a0b88f7edc5
                // This was not used for the implementation because of incompatibility with netstandard1.1
                return new Dictionary<char, char> {
                    {'\"', '\"' },
                    {'«', '»' },
                    {'‘', '’' },
                    {'“', '”' },
                    {'„', '‟' },
                    {'‹', '›' },
                    {'‚', '‛' },
                    {'《', '》' },
                    {'〈', '〉' },
                    {'「', '」' },
                    {'『', '』' },
                    {'〝', '〞' },
                    {'﹁', '﹂' },
                    {'﹃', '﹄' },
                    {'＂', '＂' },
                    {'＇', '＇' },
                    {'｢', '｣' },
                    {'(', ')' },
                    {'༺', '༻' },
                    {'༼', '༽' },
                    {'᚛', '᚜' },
                    {'⁅', '⁆' },
                    {'⌈', '⌉' },
                    {'⌊', '⌋' },
                    {'❨', '❩' },
                    {'❪', '❫' },
                    {'❬', '❭' },
                    {'❮', '❯' },
                    {'❰', '❱' },
                    {'❲', '❳' },
                    {'❴', '❵' },
                    {'⟅', '⟆' },
                    {'⟦', '⟧' },
                    {'⟨', '⟩' },
                    {'⟪', '⟫' },
                    {'⟬', '⟭' },
                    {'⟮', '⟯' },
                    {'⦃', '⦄' },
                    {'⦅', '⦆' },
                    {'⦇', '⦈' },
                    {'⦉', '⦊' },
                    {'⦋', '⦌' },
                    {'⦍', '⦎' },
                    {'⦏', '⦐' },
                    {'⦑', '⦒' },
                    {'⦓', '⦔' },
                    {'⦕', '⦖' },
                    {'⦗', '⦘' },
                    {'⧘', '⧙' },
                    {'⧚', '⧛' },
                    {'⧼', '⧽' },
                    {'⸂', '⸃' },
                    {'⸄', '⸅' },
                    {'⸉', '⸊' },
                    {'⸌', '⸍' },
                    {'⸜', '⸝' },
                    {'⸠', '⸡' },
                    {'⸢', '⸣' },
                    {'⸤', '⸥' },
                    {'⸦', '⸧' },
                    {'⸨', '⸩' },
                    {'【', '】'},
                    {'〔', '〕' },
                    {'〖', '〗' },
                    {'〘', '〙' },
                    {'〚', '〛' }
                };
            }
        }
    }
}
