using FluentAssertions;
using Xunit;
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable StringLiteralTypo

// ReSharper disable once CheckNamespace
namespace System.Collections.Specialized.Tests
{
    public class TrieTests
    {
        [Fact]
        public void Add_NullValueAsArgument_ShouldThrow()
        {
            // Arrange
            var trie = new Trie();

            // Act
            Action act = () => trie.Add(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Contains_NullValueAsArgument_ShouldThrow()
        {
            // Arrange
            var trie = new Trie();

            // Act
            Action act = () => trie.Contains(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ContainsPrefix_NullValueAsArgument_ShouldThrow()
        {
            // Arrange
            var trie = new Trie();

            // Act
            Action act = () => trie.ContainsPrefix(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NegativeInitialCapacity_ShouldThrow()
        {
            // Arrange/Act
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Trie(-1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ZeroInitialCapacity()
        {
            // Arrange/Act/Assert
            // ReSharper disable once ObjectCreationAsStatement
            new Trie(0);
        }

        [Fact]
        public void SetCapacity_GreaterThanDefault()
        {
            // Arrange
            var trie = new Trie();

            // Act
            trie.Capacity = 512;

            // Assert
            trie.Capacity.Should().Be(512);
        }

        [Fact]
        public void SetCapacity_LessThanDefault_ShouldThrow()
        {
            // Arrange
            var trie = new Trie();

            // Act
            Action act = () => trie.Capacity = 4;

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Contains()
        {
            // Arrange
            var trie = new Trie();
            trie.Add("foo");
            trie.Add("bar");
            trie.Add("footer");
            
            // Act/Assert
            trie.Contains("foo").Should().BeTrue();
            trie.Contains("bar").Should().BeTrue();
            trie.Contains("footer").Should().BeTrue();
            trie.Contains("foot").Should().BeFalse();
            trie.Contains("ba").Should().BeFalse();
        }

        [Fact]
        public void ContainsPrefix()
        {
	        // Arrange
	        var trie = new Trie();
	        trie.Add("foo");
	        trie.Add("bar");
	        trie.Add("footer");
            
	        // Act/Assert
	        trie.ContainsPrefix("f").Should().BeTrue();
	        trie.ContainsPrefix("fo").Should().BeTrue();
	        trie.ContainsPrefix("foo").Should().BeTrue();
	        trie.ContainsPrefix("foot").Should().BeTrue();
	        trie.ContainsPrefix("foote").Should().BeTrue();
	        trie.ContainsPrefix("footer").Should().BeTrue();
	        trie.ContainsPrefix("b").Should().BeTrue();
	        trie.ContainsPrefix("ba").Should().BeTrue();
	        trie.ContainsPrefix("bar").Should().BeTrue();
	        trie.ContainsPrefix("c").Should().BeFalse();
	        trie.ContainsPrefix("car").Should().BeFalse();
	        trie.ContainsPrefix("bat").Should().BeFalse();
	        trie.ContainsPrefix("fool").Should().BeFalse();
        }

        [Fact, UseCulture("tr-TR")]
        public void Contains_WithNonStandardCulture()
        {
            // Arrange
	        var trie = new Trie();
	        trie.Add("FİLE");

            // Act/Assert
            trie.Contains("FILE").Should().BeFalse();
            trie.Contains("FİLE").Should().BeTrue();
        }

        [Fact, UseCulture("tr-TR")]
        public void ContainsPrefix_WithNonStandardCulture()
        {
	        // Arrange
	        var trie = new Trie();
	        trie.Add("FİLE");

	        // Act/Assert
	        trie.ContainsPrefix("FILE").Should().BeFalse();
	        trie.ContainsPrefix("FİLE").Should().BeTrue();
	        trie.ContainsPrefix("FI").Should().BeFalse();
	        trie.ContainsPrefix("Fİ").Should().BeTrue();
	        trie.ContainsPrefix("F").Should().BeTrue();
        }
    }
}
