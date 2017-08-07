// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    internal static class Precondition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Require(bool condition)
        {
            if (!condition)
            {
                Fail();
            }
        }

        private static void Fail()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            throw new Failure();
        }

        public sealed class Failure : Exception
        {
            static string s_message = "precondition failed";
            internal Failure() : base(s_message) { }
        } 
    }
}
