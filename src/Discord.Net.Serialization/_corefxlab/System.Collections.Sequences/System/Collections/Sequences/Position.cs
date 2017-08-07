// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Collections.Sequences
{
    public struct Position : IEquatable<Position>
    {
        public object ObjectPosition;
        public int IntegerPosition;
        public int Tag;

        public static readonly Position First = new Position();
        public static readonly Position AfterLast = new Position() { IntegerPosition = int.MaxValue - 1 };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(Position left, Position right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator!=(Position left, Position right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Position other)
        {
            return IntegerPosition == other.IntegerPosition && ObjectPosition == other.ObjectPosition;
        }

        public override int GetHashCode()
        {
            return ObjectPosition == null ? IntegerPosition.GetHashCode() : ObjectPosition.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is Position)
                return base.Equals((Position)obj);
            return false;
        }
    }
}
