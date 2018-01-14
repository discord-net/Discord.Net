using System.Diagnostics;
using Model = Discord.API.Rpc.Pan;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Pan
    {
        public float Left { get; }
        public float Right { get; }

        public Pan(float left, float right)
        {
            Left = left;
            Right = right;
        }
        internal static Pan Create(Model model)
        {
            return new Pan(model.Left, model.Right);
        }

        public override string ToString() => $"Left = {Left}, Right = {Right}";
        private string DebuggerDisplay => $"Left = {Left}, Right = {Right}";
    }
}
