using Colegio;
using Colegio.Models;
using Colegio.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Student.Producer;

public class GradeTests
{
    [Fact]
    public void GetGrades_ReturnsGradeList_WhenDataExists()
    {
        // Arrange
        var data = new List<GradeDto>
        {
            new GradeDto {Id = Guid.NewGuid(),Name= "Primero",Number=1 },
            new GradeDto {Id = Guid.NewGuid(),Name= "Segundo",Number=2}
        }.AsQueryable();

        Mock<DbSet<GradeDto>> mockSet = MoqDbsetGrade(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetGrades();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    private static Mock<DbSet<GradeDto>> MoqDbsetGrade(IQueryable<GradeDto> data)
    {
        var mockSet = new Mock<DbSet<GradeDto>>();
        mockSet.As<IQueryable<GradeDto>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<GradeDto>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<GradeDto>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        mockSet.Setup(m => m.Add(It.IsAny<GradeDto>())).Callback<GradeDto>((s) => data.ToList().Add(s));
        mockSet.Setup(m => m.Remove(It.IsAny<GradeDto>())).Callback<GradeDto>((s) => data.ToList().Remove(s));
        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => data.FirstOrDefault(d => d.Id == (Guid)ids[0]));
        return mockSet;
    }

    [Fact]
    public void GetGradess_ReturnsNull()
    {
        // Arrange
        var data = new List<GradeDto>();

        Mock<DbSet<GradeDto>> mockSet = MoqDbsetGrade(data.AsQueryable());

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetGrades();

        // Assert
        Assert.Equal(data.ToList(), result);
    }

    [Fact]
    public void CreateGrade_ReturnsTrue_WhenGradeIsCreated()
    {
        // Arrange
        var grade = new GradeDto { Name = "Pimero", Number = 1 };

        var mockSet = new Mock<DbSet<GradeDto>>();
        mockSet.Setup(m => m.Add(It.IsAny<GradeDto>()));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        mockProducer.Setup(p => p.ProduceMessage(It.IsAny<string>()));
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.CreateGrade(grade);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Add(It.IsAny<GradeDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteGrade_ReturnsTrue_WhenGradeExists()
    {
        // Arrange
        var gradeId = Guid.NewGuid();
        var grade = new GradeDto { Id = gradeId, Name = "Test", Number = 0 };

        var data = new List<GradeDto> { grade }.AsQueryable();
        var mockSet = MoqDbsetGrade(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteGrade(gradeId);

        // Assert
        Assert.True(result);
        mockSet.Verify(m => m.Remove(It.IsAny<GradeDto>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void DeleteGrade_ReturnsFalse_WhenGradeDoesNotExist()
    {
        // Arrange
        var gradeId = Guid.NewGuid();

        var data = new List<GradeDto>().AsQueryable();
        var mockSet = MoqDbsetGrade(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteGrade(gradeId);

        // Assert
        Assert.False(result);
        mockSet.Verify(m => m.Remove(It.IsAny<GradeDto>()), Times.Never());
        mockContext.Verify(m => m.SaveChanges(), Times.Never());
    }

    [Fact]
    public void DeleteGrade_LogsError_WhenExceptionIsThrown()
    {
        // Arrange
        var gradeId = Guid.NewGuid();

        var mockSet = new Mock<DbSet<GradeDto>>();
        mockSet.Setup(m => m.Find(gradeId)).Throws(new Exception("Test exception"));

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.DeleteGrade(gradeId);

        // Assert
        Assert.False(result);
        mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }

    [Fact]
    public void GetGradeByNumber_ReturnsGrade_WhenGradeExists()
    {
        // Arrange
        var grade = new GradeDto { Id = Guid.NewGuid(), Name = "Primero", Number = 1 };

        var data = new List<GradeDto> { grade }.AsQueryable();
        var mockSet = MoqDbsetGrade(data);

        var mockContext = new Mock<IColegioContext>();
        mockContext.Setup(c => c.Grades).Returns(mockSet.Object);

        var mockLogger = new Mock<ILogger<Grade>>();
        var mockProducer = new Mock<IProducer>();
        var service = new Grade(mockLogger.Object, mockContext.Object, mockProducer.Object);

        // Act
        var result = service.GetGradeByNumber(grade.Number);

        // Assert
        Assert.Equal(grade, result);
    }


}