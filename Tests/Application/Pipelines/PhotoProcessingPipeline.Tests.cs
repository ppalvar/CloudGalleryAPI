namespace Tests;

using Xunit;
using Moq;
using Application.Pipelines;
using Application;
using Application.ExternalServices.Interfaces;
using System.Threading.Tasks;

public class PhotoProcessingPipeline_Tests
{
    private readonly PhotoProcessingPipeline _sut;
    private readonly Mock<IVirusScannerService> _mockVirusScannerService;
    private readonly Mock<IPhotoStorageService> _mockPhotoStorageService;
    private readonly Mock<IThumbnailGenerationService> _mockThumbnailGenerationService;
    private readonly Mock<IFileEncryptService> _mockFileEncryptService;

    public PhotoProcessingPipeline_Tests()
    {
        _mockVirusScannerService = new Mock<IVirusScannerService>();
        _mockThumbnailGenerationService = new Mock<IThumbnailGenerationService>();
        _mockFileEncryptService = new Mock<IFileEncryptService>();
        _mockPhotoStorageService = new Mock<IPhotoStorageService>();

        _sut = new PhotoProcessingPipeline(_mockVirusScannerService.Object,
                                           _mockThumbnailGenerationService.Object,
                                           _mockPhotoStorageService.Object,
                                           _mockFileEncryptService.Object);
    }

    [Fact]
    public async Task ProccessAndUpload_IfAllOK_ShouldReturnTrue()
    {
        _mockVirusScannerService.Setup(x => x.IsFileSecure(It.IsAny<byte[]>()))
                                .ReturnsAsync(() => true);
        _mockThumbnailGenerationService.Setup(x => x.GetThumbnail(It.IsAny<byte[]>()))
                                       .ReturnsAsync(new byte[10]);
        _mockFileEncryptService.Setup(x => x.GetEncryptedFile(It.IsAny<byte[]>(), default))
                               .ReturnsAsync(new byte[10]);
        _mockPhotoStorageService.Setup(x => x.SaveFile(It.IsAny<byte[]>(), It.IsAny<string>()))
                                .ReturnsAsync(() => true);

        var result = await _sut.ProccessAndUpload(new byte[10], "foo");

        Assert.True(result);
    }

    [Fact]
    public async Task ProccessAndUpload_IfVirusScanFails_ShouldReturnFalse()
    {
        _mockVirusScannerService.Setup(x => x.IsFileSecure(It.IsAny<byte[]>()))
                                .ReturnsAsync(() => false);
        _mockThumbnailGenerationService.Setup(x => x.GetThumbnail(It.IsAny<byte[]>()))
                                       .ReturnsAsync(new byte[10]);
        _mockFileEncryptService.Setup(x => x.GetEncryptedFile(It.IsAny<byte[]>(), default))
                               .ReturnsAsync(new byte[10]);
        _mockPhotoStorageService.Setup(x => x.SaveFile(It.IsAny<byte[]>(), It.IsAny<string>()))
                                .ReturnsAsync(() => true);

        var result = await _sut.ProccessAndUpload(new byte[10], "foo");

        Assert.False(result);
    }

    [Theory]
    [InlineData("foo", false)]
    [InlineData("foo", true)]
    public async Task RetrieveImage_Always_ShouldReturnTheImage(string filename, bool isThumbnail)
    {
        byte[] mockImage = new byte[10];
        
        _mockFileEncryptService.Setup(x => x.GetDecryptedFile(It.IsAny<byte[]>(), default))
                               .ReturnsAsync(mockImage);
        _mockPhotoStorageService.Setup(x => x.RetrieveFile(It.IsAny<string>()))
                                .ReturnsAsync(mockImage);

        var result = await _sut.RetrieveImage(filename, isThumbnail);

        Assert.Equal(mockImage, result);
    }
}