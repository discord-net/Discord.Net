using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests for the <see cref="Discord.Color"/> type.
    /// </summary>
    public class ColorTests
    {
        [Fact]
        public void Color_New()
        {
            Assert.Equal(0u, new Color().RawValue);
            Assert.Equal(uint.MinValue, new Color(uint.MinValue).RawValue);
            Assert.Throws<ArgumentException>(() => new Color(uint.MaxValue));
        }
        [Fact]
        public void Color_Default()
        {
            Assert.Equal(0u, Color.Default.RawValue);
            Assert.Equal(0, Color.Default.R);
            Assert.Equal(0, Color.Default.G);
            Assert.Equal(0, Color.Default.B);
        }
        [Fact]
        public void Color_FromRgb_Byte()
        {
            Assert.Equal(0xFF0000u, new Color((byte)255, (byte)0, (byte)0).RawValue);
            Assert.Equal(0x00FF00u, new Color((byte)0, (byte)255, (byte)0).RawValue);
            Assert.Equal(0x0000FFu, new Color((byte)0, (byte)0, (byte)255).RawValue);
            Assert.Equal(0xFFFFFFu, new Color((byte)255, (byte)255, (byte)255).RawValue);
        }
        [Fact]
        public void Color_FromRgb_Int()
        {
            Assert.Equal(0xFF0000u, new Color(255, 0, 0).RawValue);
            Assert.Equal(0x00FF00u, new Color(0, 255, 0).RawValue);
            Assert.Equal(0x0000FFu, new Color(0, 0, 255).RawValue);
            Assert.Equal(0xFFFFFFu, new Color(255, 255, 255).RawValue);
        }
        [Fact]
        public void Color_FromRgb_Int_OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>("r", () => new Color(-1024, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("r", () => new Color(1024, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("g", () => new Color(0, -1024, 0));
            Assert.Throws<ArgumentOutOfRangeException>("g", () => new Color(0, 1024, 0));
            Assert.Throws<ArgumentOutOfRangeException>("b", () => new Color(0, 0, -1024));
            Assert.Throws<ArgumentOutOfRangeException>("b", () => new Color(0, 0, 1024));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Color(-1024, -1024, -1024));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Color(1024, 1024, 1024));
        }
        [Fact]
        public void Color_FromRgb_Float()
        {
            Assert.Equal(0xFF0000u, new Color(1.0f, 0, 0).RawValue);
            Assert.Equal(0x00FF00u, new Color(0, 1.0f, 0).RawValue);
            Assert.Equal(0x0000FFu, new Color(0, 0, 1.0f).RawValue);
            Assert.Equal(0xFFFFFFu, new Color(1.0f, 1.0f, 1.0f).RawValue);
        }
        [Fact]
        public void Color_FromRgb_Float_OutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>("r", () => new Color(-2.0f, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("r", () => new Color(2.0f, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>("g", () => new Color(0, -2.0f, 0));
            Assert.Throws<ArgumentOutOfRangeException>("g", () => new Color(0, 2.0f, 0));
            Assert.Throws<ArgumentOutOfRangeException>("b", () => new Color(0, 0, -2.0f));
            Assert.Throws<ArgumentOutOfRangeException>("b", () => new Color(0, 0, 2.0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Color(-2.0f, -2.0f, -2.0f));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Color(2.0f, 2.0f, 2.0f));
        }
        [Fact]
        public void Color_Red()
        {
            Assert.Equal(0xAF, new Color(0xAF1390).R);
        }
        [Fact]
        public void Color_Green()
        {
            Assert.Equal(0x13, new Color(0xAF1390).G);
        }
        [Fact]
        public void Color_Blue()
        {
            Assert.Equal(0x90, new Color(0xAF1390).B);
        }
    }
}
