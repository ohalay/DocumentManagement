using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DocumentManagement.Core.Domain;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DocumentManagement.DocumentStore.Blob.Tests
{
    public class BlobDocumentStoreTest : IClassFixture<BlobFixture>
    {
        private readonly Fixture fixture = new Fixture();
        private readonly BlobDocumentStore store;
        private readonly string containerUri;

        public BlobDocumentStoreTest(BlobFixture fixture)
        {
            var option = fixture.BlobOptions;
            store = new BlobDocumentStore(Options.Create(option));
            containerUri = $"{fixture.BlobServiceUri}/{option.ContainerPrefix}{option.ContainerName}";
        }

        [Fact]
        public async Task UploadAsync_PdfDocument_DocumentUploaded()
        {
            var name = fixture.Create<string>();
            var content = fixture.Create<string>();

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            using var stream = new MemoryStream(byteArray);

            var result = await store.UploadAsync(name, stream);

            result.Successful.Should().BeTrue();
        }

        [Fact]
        public async Task UploadAsync_TheSameDocumentName_DocumentUploaded()
        {
            var name = fixture.Create<string>();
            var content = fixture.Create<string>();

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            using var stream1 = new MemoryStream(byteArray);
            using var stream2 = new MemoryStream(byteArray);

            await store.UploadAsync(name, stream1);
            var result = await store.UploadAsync(name, stream2);

            result.Successful.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_TwoDocuments_DocumentsRetrieved()
        {
            var content = fixture.Create<string>();

            var entities = fixture.CreateMany<string>(2)
                .Select(name => DocumentEntity.Create(name, content.Length, new Uri($"{containerUri}/{name}")))
                .Select(s => s.entity)
                .ToArray();

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            using var stream1 = new MemoryStream(byteArray);
            using var stream2 = new MemoryStream(byteArray);

            await store.UploadAsync(entities[0].Name, stream1);
            await store.UploadAsync(entities[1].Name, stream2);

            var result = await store.GetAllAsync();

            result.Should().BeEquivalentTo(entities);
        }

        [Fact]
        public async Task GetAllAsync_NoDocuments_EmptyListRetrieved()
        {
            var result = await store.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ReorderAsync_SigleDocument_DocumentReordered()
        {
            var name = fixture.Create<string>();
            var content = fixture.Create<string>();

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            using var stream = new MemoryStream(byteArray);

            var (_, entity) = DocumentEntity.Create(name, content.Length, new Uri($"{containerUri}/{name}"), 5);

            await store.UploadAsync(name, stream);
            await store.ReorderAsync(entity);

            var result = await store.GetAllAsync();

            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public async Task ReorderAsync_NoDocument_ErrorResultReturned()
        {
            var name = fixture.Create<string>();
            var content = fixture.Create<string>();

            var (_, entity) = DocumentEntity.Create(name, content.Length, new Uri($"{containerUri}/{name}"), 5);

            var result = await store.ReorderAsync(entity);

            result.Successful.Should().BeFalse();
        }

        [Fact]
        public async Task DaleteAsync_SigleDocument_DocumentDeleted()
        {
            var name = fixture.Create<string>();
            var content = fixture.Create<string>();

            byte[] byteArray = Encoding.ASCII.GetBytes(content);
            using var stream = new MemoryStream(byteArray);

            await store.UploadAsync(name, stream);
            await store.DeleteAsync(name);

            var result = await store.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DaleteAsync_NoDocument_ErrorResultReturned()
        {
            var name = fixture.Create<string>();
            var result = await store.DeleteAsync(name);

            result.Successful.Should().BeFalse();
        }
    }
}