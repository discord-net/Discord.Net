// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text
{
    internal static class ParsingTrie
    {
        #region Parsing trie struct

        // The parsing trie is structured as an array, which means that there are two types of
        // "nodes" for representational purposes
        //
        // The first node type (the parent node) uses the valueOrNumChildren to represent the number of children
        // underneath it. The index is unused for this type of node, except when it's used for
        // sequential node mapping (see below). If valueOrNumChildren is zero for this type of node, the index
        // is used and represents an index into _digitsAndSymbols.
        //
        // The second node types immediately follow the first (the childe nodes). They are composed of a value
        // (valueOrNumChildren), which is walked via binary search, and an index, which points to another
        // node contained in the array.
        //
        // We use the int index here to encode max-min info for sequential leaves
        // It's very common for digits to be encoded sequentially, so we save time by mapping here
        // The index is formatted as such: 0xAABBCCDD, where AA = the min value,
        // BB = the index of the min value relative to the current node (1-indexed),
        // CC = the max value, and DD = the max value's index in the same coord-system as BB.
        public struct Node
        {
            public byte ValueOrNumChildren;
            public int IndexOrSymbol;
        }

        #endregion Parsing trie struct

        /// <summary>
        /// A Suffix represents the ending sequence of bytes that correspond to a symbol.
        /// Suffixes play an important role in the parsing trie generation algorithm.
        /// 
        /// Let's say there are four symbols:
        /// Symbol 0: Sequence 1, 1, 2, 3
        /// Symbol 1: Sequence 0, 1, 2, 3
        /// Symbol 2: Sequence 0, 1, 4, 4
        /// Symbol 3: Sequence 1, 1, 2, 1
        /// 
        /// First, a Suffix is created for each symbol's sequence, and the Suffixes are sorted by their byte sequences:
        /// ListOfSuffix {
        ///		Suffix { SymbolIndex: 1, Bytes: { 0, 1, 2, 3 } }
        ///		Suffix { SymbolIndex: 2, Bytes: { 0, 1, 4, 4 } }
        ///		Suffix { SymbolIndex: 3, Bytes: { 1, 1, 2, 1 } }
        ///		Suffix { SymbolIndex: 0, Bytes: { 1, 1, 2, 3 } }
        /// }
        /// 
        /// Next, the Suffixes are clumped into SuffixClumps, based on the beginning byte:
        /// ListOfSuffixClump {
        ///		SuffixClump {
        ///			BeginningByte: 0
        ///			Suffixes {
        ///				Suffix { SymbolIndex: 1, Bytes: { 1, 2, 3 } }
        ///				Suffix { SymbolIndex: 2, Bytes: { 1, 4, 4 } }
        ///			}
        ///		}
        ///		SuffixClump {
        ///			BeginningByte: 1
        ///			Suffixes {
        ///				Suffix { SymbolIndex: 3, Bytes: { 1, 2, 1 } }
        ///				Suffix { SymbolIndex: 0, Bytes: { 1, 2, 3 } }
        ///			}
        ///		}
        ///	}
        ///	
        /// Then, a parent ParsingTrieNode is created, with its NumChildren equal to the number of SuffixClumps.
        /// Each SuffixClump represents both a "child" node in the parsing trie, and the "parent" node that child
        /// node points to.
        /// 
        /// Each SuffixClump that has more than one Suffix will require further clumping; that is to say, it does
        /// not represent a leaf node in the parsing trie. Such SuffixClumps will be recursively clumped.
        /// </summary>
        private struct Suffix : IComparable<Suffix>
        {
            public int SymbolIndex;
            public byte[] Bytes;

            public Suffix(int symbolIndex, byte[] bytes)
            {
                SymbolIndex = symbolIndex;
                Bytes = bytes;
            }

            public Suffix(int symbolIndex, ReadOnlySpan<byte> bytes)
            {
                SymbolIndex = symbolIndex;

                // HACKHACK: Keeping Bytes as a Span property on Suffix will cause crashing in .NET Core 2.0.
                //           Storing as pure array for now until we can re-visit.
                //           This is necessary to unblock usage of fast Span for Kestrel and others.
                Bytes = bytes.ToArray();
            }

            public int CompareTo(Suffix other)
            {
                var shorter = Math.Min(other.Bytes.Length, Bytes.Length);
                for(int index = 0; index < shorter; index++)
                {
                    if (Bytes[index] == other.Bytes[index]) continue;
                    return Bytes[index].CompareTo(other.Bytes[index]);
                }
                return Bytes.Length.CompareTo(other.Bytes.Length);
                
            }
        }

        private struct SuffixClump
        {
            public byte BeginningByte;
            public List<Suffix> Suffixes;

            public SuffixClump(byte beginningByte)
            {
                BeginningByte = beginningByte;
                // This list of suffixes will not exceed the number of symbols. Initialize
                // the list to be of size 20, which is slightly larger than the number of symbols.
                Suffixes = new List<Suffix>(20);
            }
        }

        private struct Sequence : IComparable<Sequence>
        {
            public int BeginningIndex;
            public int EndIndex;
            public byte BeginningValue;
            public byte EndValue;

            // This constructor creates a sequence of length 0.
            public Sequence(int index, byte value)
            {
                BeginningIndex = index;
                EndIndex = index;
                BeginningValue = value;
                EndValue = value;
            }

            public int CompareTo(Sequence other)
            {
                int thisLength = EndIndex - BeginningIndex;
                int otherLength = other.EndIndex - other.BeginningIndex;
                return thisLength.CompareTo(otherLength);
            }

            public int Length
            {
                get
                {
                    return EndIndex - BeginningIndex;
                }
            }

            // Sequence map is formatted as such:
            // 0xAABBCCDD
            // AA: The min value
            // BB: The index of the min value relative to the current node (1-indexed)
            // CC: The max value
            // DD: The max value's index in the same coord-system as BB
            public int CreateSequenceMap()
            {
                int sequenceMap = 0;
                // AA
                sequenceMap += BeginningValue << 24;
                // BB: Add 1 to BeginningIndex because the parent node is located 1 place before the 0-indexed child node
                sequenceMap += (BeginningIndex + 1) << 16;
                // CC
                sequenceMap += EndValue << 8;
                // DD: Add 1 to EndIndex for same reason as BB
                sequenceMap += EndIndex + 1;

                return sequenceMap;
            }
        }

        // The return value here is the index in parsingTrieList at which the parent node was placed.
        private static int CreateParsingTrieNodeAndChildren(ref List<Node> parsingTrieList, List<Suffix> sortedSuffixes)
        {
            // If there is only one suffix, create a leaf node
            if (sortedSuffixes.Count == 1)
            {
                Node leafNode = new Node();
                leafNode.ValueOrNumChildren = 0;
                leafNode.IndexOrSymbol = sortedSuffixes[0].SymbolIndex;
                int leafNodeIndex = parsingTrieList.Count;
                parsingTrieList.Add(leafNode);
                return leafNodeIndex;
            }

            // Group suffixes into clumps based on first byte
            List<SuffixClump> clumps = new List<SuffixClump>(sortedSuffixes.Count);
            byte beginningByte = sortedSuffixes[0].Bytes[0];
            SuffixClump currentClump = new SuffixClump(beginningByte);
            clumps.Add(currentClump);

            // Initialize sequence detection
            Sequence currentSequence = new Sequence(0, beginningByte);
            Sequence longestSequence = currentSequence;

            foreach (Suffix suffix in sortedSuffixes)
            {
                var bytesSpan = new Span<byte>(suffix.Bytes);
                if (suffix.Bytes[0] == beginningByte)
                {
                    currentClump.Suffixes.Add(new Suffix(suffix.SymbolIndex, bytesSpan.Slice(1)));
                }
                else
                {
                    beginningByte = suffix.Bytes[0];

                    // Determine if the new clump is part of a sequence
                    if (beginningByte == currentSequence.EndValue + 1)
                    {
                        // This clump is part of the current sequence
                        currentSequence.EndIndex++;
                        currentSequence.EndValue++;

                        if (!currentSequence.Equals(longestSequence) && currentSequence.CompareTo(longestSequence) > 0)
                        {
                            // Replace the longest sequence with this sequence
                            longestSequence = currentSequence;
                        }
                    }
                    else
                    {
                        // This clump is part of a new sequence
                        currentSequence = new Sequence(clumps.Count, beginningByte);
                    }

                    // This is a new clump, with at least one suffix inside it. Add to the list of clumps.
                    currentClump = new SuffixClump(beginningByte);
                    currentClump.Suffixes.Add(new Suffix(suffix.SymbolIndex, bytesSpan.Slice(1)));
                    clumps.Add(currentClump);
                }
            }

            // Now that we know how many children there are, create parent node and place in list
            Node parentNode = new Node();
            parentNode.ValueOrNumChildren = (byte)clumps.Count;
            // Only bother specifying a sequence if the longest sequence is sufficiently long
            if (longestSequence.Length > 5)
            {
                parentNode.IndexOrSymbol = longestSequence.CreateSequenceMap();
            }
            else
            {
                parentNode.IndexOrSymbol = 0;
            }
            int parentNodeIndex = parsingTrieList.Count;
            parsingTrieList.Add(parentNode);

            // Reserve space in list for child nodes. In this algorithm, all parent nodes are created first, leaving gaps for the child nodes
            // to be filled in once it is known where they point to.
            int childNodeStartIndex = parsingTrieList.Count;
            for (int i = 0; i < clumps.Count; i++)
            {
                parsingTrieList.Add(default);
            }

            // Process child nodes
            List<Node> childNodes = new List<Node>();
            foreach (SuffixClump clump in clumps)
            {
                Node childNode = new Node();
                childNode.ValueOrNumChildren = clump.BeginningByte;
                childNode.IndexOrSymbol = CreateParsingTrieNodeAndChildren(ref parsingTrieList, clump.Suffixes);
                childNodes.Add(childNode);
            }

            // Place child nodes in spots allocated for them
            int childNodeIndex = childNodeStartIndex;
            foreach (Node childNode in childNodes)
            {
                parsingTrieList[childNodeIndex] = childNode;
                childNodeIndex++;
            }

            return parentNodeIndex;
        }

        public static Node[] Create(byte[][] symbols)
        {
            List<Suffix> symbolList = new List<Suffix>(symbols.Length);
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i] != null)
                {
                    symbolList.Add(new Suffix(i, symbols[i]));
                }
            }

            // Sort the symbol list. This is important for allowing binary search of the child nodes, as well as
            // counting the number of children a node has.
            symbolList.Sort();

            // validate symbol consistemcy:
            //   a) each symbol must be unique
            //   b) a symbol cannot be a prefix of another symbol
            //   c) symbols cannot be empty
            for(int i = 1; i < symbolList.Count; i++)
            {
                var first = symbolList[i - 1];
                var second = symbolList[i];

                if(first.Bytes.Length == 0 || second.Bytes.Length == 0)
                {
                    throw new ArgumentException("Symbol cannot be zero bytes long");
                }
                var firstSpan = first.Bytes.AsSpan();
                if (firstSpan.SequenceEqual(second.Bytes))
                {
                    throw new ArgumentException("Symbols cannot be identical");
                }
                if (first.Bytes.Length > second.Bytes.Length)
                {
                    if (firstSpan.StartsWith(second.Bytes))
                    {
                        throw new ArgumentException("Symbols are ambiguous");
                    }
                }
                else if(first.Bytes.Length < second.Bytes.Length)
                {
                    if (second.Bytes.AsSpan().StartsWith(first.Bytes))
                    {
                        throw new ArgumentException("Symbols are ambiguous");
                    }
                }
            }

            List<Node> parsingTrieList = new List<Node>(100);
            CreateParsingTrieNodeAndChildren(ref parsingTrieList, symbolList);

            return parsingTrieList.ToArray();
        }
    }
}
