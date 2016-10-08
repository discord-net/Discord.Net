using Model = Discord.API.Rpc.Pan;

namespace Discord.Rpc
{
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
    }
}
