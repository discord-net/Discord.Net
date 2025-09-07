// using Microsoft.CodeAnalysis.Text;
// using System.Collections.Generic;
//
// namespace Discord.ComponentDesignerGenerator.Parser;
//
// public sealed class CXBlender
// {
//     public int CurrentSourcePosition => _lexer.Reader.Position;
//
//     private readonly CXLexer _lexer;
//     private readonly CXDoc _doc;
//     private readonly Queue<TextChangeRange> _changes;
//     private readonly List<CXNode> _nodes;
//     private readonly List<CXToken> _tokens;
//
//     private int _changeDelta;
//     private int _docTokenIndex;
//
//     public CXBlender(
//         CXLexer lexer,
//         CXDoc doc,
//         IEnumerable<TextChangeRange> changes,
//         List<CXNode> nodes,
//         List<CXToken> tokens
//     )
//     {
//         _lexer = lexer;
//         _doc = doc;
//         _changes = new(changes);
//         _nodes = nodes;
//         _tokens = tokens;
//     }
//
//     public CXToken GetToken(int index)
//     {
//         if (_tokens.Count > index)
//             return _tokens[index];
//
//         while (_tokens.Count <= index)
//         {
//             var token = NextToken();
//
//             if (token.Kind is CXTokenKind.EOF) return token;
//         }
//
//         return _tokens[index];
//     }
//
//     public CXToken NextToken()
//     {
//         SkipPastChanges();
//
//         while (true)
//         {
//             while (_changeDelta < 0 && _docTokenIndex < _doc.Tokens.Count)
//             {
//                 var oldToken = _doc.Tokens[_docTokenIndex++];
//                 _changeDelta += oldToken.AbsoluteWidth;
//             }
//
//             if (_changeDelta > 0) return LexNewToken();
//
//             if (TryReuseToken(out var token)) return token;
//
//             if (_doc.Tokens.Count <= _docTokenIndex) return LexNewToken();
//
//             _changeDelta += _doc.Tokens[_docTokenIndex++].AbsoluteWidth;
//         }
//
//         bool TryReuseToken(out CXToken token)
//         {
//             if (_docTokenIndex >= _doc.Tokens.Count)
//             {
//                 token = default;
//                 return false;
//             }
//
//             token = _doc.Tokens[_docTokenIndex];
//
//             if (!CanReuse(token)) return false;
//
//             _lexer.Reader.Advance(token.AbsoluteWidth);
//             _tokens.Add(token);
//             return true;
//         }
//     }
//
//     private CXToken LexNewToken()
//     {
//         while (true)
//         {
//             var token = _lexer.Next();
//
//             if(token.Kind is CXTokenKind.Invalid) continue;
//
//             _tokens.Add(token);
//             _changeDelta += token.AbsoluteWidth;
//             return token;
//         }
//     }
//
//     private bool CanReuse(CXToken token)
//     {
//         if (token.Span.Length is 0) return false;
//
//         if (IntersectsNextChange(token.Span)) return false;
//
//         return true;
//     }
//
//     private bool IntersectsNextChange(TextSpan span)
//     {
//         if (_changes.Count is 0) return false;
//
//         return span.IntersectsWith(_changes.Peek().Span);
//     }
//
//     private void SkipPastChanges()
//     {
//         while (_changes.Count is not 0)
//         {
//             var change = _changes.Peek();
//
//             if (change.Span.Start + change.NewLength > CurrentSourcePosition)
//                 break;
//
//             _changes.Dequeue();
//
//             _changeDelta += change.NewLength - change.Span.Length;
//
//             while (_docTokenIndex < _doc.Tokens.Count)
//             {
//                 var token = _doc.Tokens[_docTokenIndex];
//
//                 if (token.AbsoluteStart >= change.Span.Start)
//                     break;
//
//                 _docTokenIndex++;
//             }
//         }
//     }
// }
