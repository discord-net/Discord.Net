namespace Discord
{
    public struct GameAssets
    {
        public string SmallText { get; }
        public string SmallImage { get; }
        public string LargeText { get; }
        public string LargeImage { get; }

        public GameAssets(string smallText, string smallImage, string largeText, string largeImage)
        {
            SmallText = smallText;
            SmallImage = smallImage;
            LargeText = largeText;
            LargeImage = largeImage;
        }
    }
}