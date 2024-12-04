using FakeItEasy;
using KeystoreDB.Core.Interfaces;

namespace KeystoreDB.Core.Tests;

public class DatabaseServiceInterfaceTests
{
    [Fact]
    public void Set_ShouldInvokeSetMethodWithCorrectParameters()
    {
        // Arrange
        var fakeDatabaseService = A.Fake<IDatabaseService>();
        var testKey = "testKey";
        var testValue = "testValue";

        // Act
        fakeDatabaseService.Set(testKey, testValue);

        // Assert
        A.CallTo(() => fakeDatabaseService.Set(testKey, testValue)).MustHaveHappenedOnceExactly();
    }
}