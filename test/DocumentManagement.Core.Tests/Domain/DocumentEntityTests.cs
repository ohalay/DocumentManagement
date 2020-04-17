using DocumentManagement.Core.Domain;
using FluentAssertions;
using Xunit;

namespace DocumentManagement.Core.Tests.Domain
{
    public class DocumentEntityTests
    {
        private const long ValidFileSize = 1;
        private const string ValidFileName = "file.pdf";

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
            var (result, _) = DocumentEntity.Create(ValidFileName, size, null);

            result.Successful.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(ValidFileName, ValidFileSize)]
        [InlineData("1.PDF", ValidFileSize)]

        public void Create_Successful_EntityCreated(string fileName, long fileSize)
        {
            var (result, entity) = DocumentEntity.Create(fileName, fileSize, null);

            result.Successful.Should().BeTrue();
            entity.Name.Should().BeEquivalentTo(fileName);
            entity.FileSize.Should().Be(fileSize);
            entity.Order.Should().Be(0);
            entity.Location.Should().BeNull();
        }
    }
}
