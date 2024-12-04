using FakeItEasy;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Core.Tests;

public class FileServiceInterfaceTests
{
    [Fact]
    public void IFileService_WriteAllString_ShouldWriteStringToFile()
    {
        // Arrange
        var fakeFileService = A.Fake<IFileService>();
        var filePath = "test_file.txt";
        var content = "This is a test content.";

        A.CallTo(() => fakeFileService.WriteAllString(A<string>._, A<string>._))
            .Invokes((string path, string data) => { File.WriteAllText(path, data); });

        // Act
        fakeFileService.WriteAllString(filePath, content);

        // Assert
        A.CallTo(() => fakeFileService.WriteAllString(filePath, content)).MustHaveHappened();
        Assert.True(File.Exists(filePath));
        var savedContent = File.ReadAllText(filePath);
        Assert.Equal(content, savedContent);

        // Cleanup
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    [Fact]
    public void IFileService_ReadAllString_ShouldReadStringFromFile()
    {
        // Arrange
        var fakeFileService = A.Fake<IFileService>();
        var filePath = @"/temp/abcd.txt";
        var expectedContent = "This is the content of the file.";
        A.CallTo(() => fakeFileService.ReadAllString(A<string>._)).Returns(expectedContent);

        // Act
        var actualContent = fakeFileService.ReadAllString(filePath);

        // Assert
        A.CallTo(() => fakeFileService.ReadAllString(filePath)).MustHaveHappened();
        Assert.Equal(expectedContent, actualContent);
    }


    [Fact]
    public void IFileService_Exists_ShouldReturnTrueForExistingFile()
    {
        // Arrange
        var fakeFileService = A.Fake<IFileService>();
        var filePath = @"/ad/ff.txt";

        A.CallTo(() => fakeFileService.Exists(filePath)).Returns(true);

        // Act
        var fileExists = fakeFileService.Exists(filePath);

        // Assert
        A.CallTo(() => fakeFileService.Exists(filePath)).MustHaveHappened();
        Assert.True(fileExists);
    }
}