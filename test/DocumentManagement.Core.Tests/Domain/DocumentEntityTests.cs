using AutoFixture;
using DocumentManagement.Core.Domain;
using FluentAssertions;
using Xunit;

namespace DocumentManagement.Core.Tests.Domain
{
    public class DocumentEntityTests
    {
        private const long ValidFileSize = 1;
        private readonly Fixture fixture = new Fixture();

        [Theory]
        [InlineData("non-pdf-file.png")]
        [InlineData("`@invalid-character.pdf")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(@"\invalid-character.pdf")]
        public void Create_InvalidName_ErrorReturned(string name)
        {
            var (result, _) = DocumentEntity.Create(name, ValidFileSize, null);

            result.Successful.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5242881)]
        public void Create_InvalidFileSize_ErrorReturned(long size)
        {
            var name = fixture.Create<string>();

            var (result, _) = DocumentEntity.Create(name, size, null);

            result.Successful.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void Create_Successful_EntityCreated()
        {
            var name = new Fixture().Create<string>();

            var (result, entity) = DocumentEntity.Create(name, ValidFileSize, null);

            result.Successful.Should().BeTrue();
            entity.Name.Should().BeEquivalentTo(name);
            entity.FileSize.Should().Be(ValidFileSize);
            entity.Order.Should().Be(0);
            entity.Location.Should().BeNull();
        }
    }
}
