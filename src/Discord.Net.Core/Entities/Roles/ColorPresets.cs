using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]

    public struct ColorPresets
    {
        /// <summary> Gets the default user color value. </summary>
        public static readonly Color Default = new Color(0);

        /// <summary> Gets the default Sea Green color value (26, 188, 156). </summary>
        public static readonly Color SeaGreen = new Color(26, 188, 156);

        /// <summary> Gets the default Shamrock Green color value (46, 204, 113). </summary>
        public static readonly Color ShamrockGreen = new Color(46, 204, 113);

        /// <summary> Gets the default Curious Blue color value (52, 152, 219). </summary>
        public static readonly Color CuriousBlue = new Color(52, 152, 219);

        /// <summary> Gets the default Purple color value (155, 89, 182). </summary>
        public static readonly Color Purple = new Color(155, 89, 182);

        /// <summary> Gets the default Pink color value (233, 30, 99). </summary>
        public static readonly Color Pink = new Color(233, 30, 99);

        /// <summary> Gets the default Buttercup Yellow color value (241, 196, 15). </summary>
        public static readonly Color ButtercupYellow = new Color(241, 196, 15);

        /// <summary> Gets the default Orange color value (230, 126, 34). </summary>
        public static readonly Color Orange = new Color(230, 126, 34);

        /// <summary> Gets the default Cinnabar Red color value (231, 76, 60). </summary>
        public static readonly Color CinnabarRed = new Color(231, 76, 60);

        /// <summary> Gets the default Cascade Grey color value (149, 165, 166). </summary>
        public static readonly Color CascadeGrey = new Color(149, 165, 166);

        /// <summary> Gets the default Light Slate Grey color value (96, 125, 139). </summary>
        public static readonly Color LightSlateGrey = new Color(96, 125, 139);

        /// <summary> Gets the default Dark Sea Green color value (17, 128, 106). </summary>
        public static readonly Color DarkSeaGreen = new Color(17, 128, 106);

        /// <summary> Gets the default Forest Green color value (31, 139, 76). </summary>
        public static readonly Color ForestGreen = new Color(31, 139, 76);

        /// <summary> Gets the default Midnight Blue color value (32, 102, 148). </summary>
        public static readonly Color MidnightBlue = new Color(32, 102, 148);

        /// <summary> Gets the default Violet color value (113, 54, 138). </summary>
        public static readonly Color Violet = new Color(113, 54, 138);

        /// <summary> Gets the default Rose color value (173, 20, 87). </summary>
        public static readonly Color Rose = new Color(173, 20, 87);

        /// <summary> Gets the default Burnt Sienna color value (194, 124, 14). </summary>
        public static readonly Color BurntSienna = new Color(194, 124, 14);

        /// <summary> Gets the default Light Brown color value (168, 67, 0). </summary>
        public static readonly Color LightBrown = new Color(168, 67, 0);

        /// <summary> Gets the default Dark Brown color value (153, 45, 34). </summary>
        public static readonly Color DarkBrown = new Color(153, 45, 34);

        /// <summary> Gets the default Grey color value (151, 156, 159). </summary>
        public static readonly Color Grey = new Color(151, 156, 159);

        /// <summary> Gets the default Dark Slate Grey color value (84, 110, 122). </summary>
        public static readonly Color DarkSlateGrey = new Color(84, 110, 122);
    }
}
