using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Discord.Serialization;

namespace Discord.Tests.Unit.Serialization
{
    public class OptionalConverterTests
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public OptionalConverterTests()
        {
            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.Converters.Add(new OptionalConverter());
        }

        public class SampleOptionalClass
        {
            [JsonPropertyName("optional_number")]
            public Optional<int> OptionalNumber { get; set; }
            [JsonPropertyName("required_number")]
            public int RequiredNumber { get; set; }

            public override bool Equals(object obj)
                => (obj is SampleOptionalClass other) && (other.OptionalNumber == OptionalNumber && other.RequiredNumber == RequiredNumber);
            public override int GetHashCode()
                => OptionalNumber.GetHashCode() ^ RequiredNumber.GetHashCode();
        }

        private string expectedOptionalUnset = "{\"optional_number\":null,\"required_number\":10}";
        private SampleOptionalClass withOptionalUnset = new SampleOptionalClass
        {
            OptionalNumber = Optional<int>.Unspecified,
            RequiredNumber = 10,
        };
        private string expectedOptionalSet = "{\"optional_number\":11,\"required_number\":10}";
        private SampleOptionalClass withOptionalSet = new SampleOptionalClass
        {
            OptionalNumber = new Optional<int>(11),
            RequiredNumber = 10,
        };

        [Fact]
        public void OptionalConverter_Can_Write()
        {
            // todo: is STJ deterministic in writing order? want to make sure this test doesn't fail because of cosmic rays
            var unsetString = JsonSerializer.Serialize(withOptionalUnset, _jsonOptions);
            Assert.Equal(expectedOptionalUnset, unsetString);
            
            var setString = JsonSerializer.Serialize(withOptionalSet, _jsonOptions);
            Assert.Equal(expectedOptionalSet, setString);
        }

        [Fact]
        public void OptionalConverter_Can_Read()
        {
            var unset = JsonSerializer.Deserialize<SampleOptionalClass>(expectedOptionalUnset, _jsonOptions);
            Assert.Equal(withOptionalUnset, unset);

            var set = JsonSerializer.Deserialize<SampleOptionalClass>(expectedOptionalSet, _jsonOptions);
            Assert.Equal(withOptionalSet, set);
        }

        public class NestedPoco
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("age")]
            public int Age { get; set; }

            public override bool Equals(object obj)
                => (obj is NestedPoco other) && (Name == other.Name && Age == other.Age);
            public override int GetHashCode()
                => Name.GetHashCode() ^ Age.GetHashCode();
            
        }
        public class NestedSampleClass
        {
            [JsonPropertyName("nested")]
            public Optional<NestedPoco> Nested { get; set; }
        }

        private string expectedNestedWithUnset = "{\"nested\":null}";
        private NestedSampleClass nestedWithUnset = new NestedSampleClass
        {
            Nested = Optional<NestedPoco>.Unspecified
        };
        private string expectedNestedWithSet = "{\"nested\":{\"name\":\"Ashley\",\"age\":23}}";
        private NestedSampleClass nestedWithSet = new NestedSampleClass
        {
            Nested = new Optional<NestedPoco>(new NestedPoco
            {
                Name = "Ashley",
                Age = 23
            }),
        };

        [Fact]
        public void OptionalConverter_Can_Write_Nested_Poco()
        {
            var unset = JsonSerializer.Serialize(nestedWithUnset, _jsonOptions);
            Assert.Equal(expectedNestedWithUnset, unset);

            var set = JsonSerializer.Serialize(nestedWithSet, _jsonOptions);
            Assert.Equal(expectedNestedWithSet, set);
        }
        [Fact]
        public void OptionalConverter_Can_Read_Nested_Poco()
        {
            var unset = JsonSerializer.Deserialize<NestedSampleClass>(expectedNestedWithUnset, _jsonOptions);
            Assert.Equal(nestedWithUnset.Nested, unset.Nested);

            var set = JsonSerializer.Deserialize<NestedSampleClass>(expectedNestedWithSet, _jsonOptions);
            Assert.Equal(nestedWithSet.Nested, set.Nested);
        }
    }
}
