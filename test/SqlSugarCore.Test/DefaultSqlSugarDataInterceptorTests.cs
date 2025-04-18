using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;
using YayZent.Framework.SqlSugarCore;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace SqlSugarCore.Test;

public class DefaultSqlSugarDataInterceptorTests
{
    private readonly DefaultSqlSugarDataInterceptor _context;
    private readonly Mock<IAbpLazyServiceProvider> _lazyServiceProviderMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<IOptions<DbConnOptions>> _dbConnOptionsMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    
    public DefaultSqlSugarDataInterceptorTests()
    {
        _lazyServiceProviderMock = new Mock<IAbpLazyServiceProvider>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _dbConnOptionsMock = new Mock<IOptions<DbConnOptions>>();
        _currentUserMock = new Mock<ICurrentUser>();

        _dbConnOptionsMock.Setup(x => x.Value).Returns(new DbConnOptions { EnableSqlLog = true });

        _lazyServiceProviderMock
            .Setup(x => x.LazyGetRequiredService<IOptions<DbConnOptions>>())
            .Returns(_dbConnOptionsMock.Object);
        _lazyServiceProviderMock
            .Setup(x => x.LazyGetRequiredService<ILoggerFactory>())
            .Returns(_loggerFactoryMock.Object);
        _lazyServiceProviderMock
            .Setup(x => x.LazyGetRequiredService<ICurrentUser>())
            .Returns(_currentUserMock.Object);

        _context = new DefaultSqlSugarDataInterceptor(_lazyServiceProviderMock.Object);
    }
    
    [Fact]
    public void OnSqlExecuting_ShouldLogSql_WhenLoggingIsEnabled()
    {
        // Arrange
        string sql = "SELECT * FROM Users";
        SugarParameter[] parameters = [];

        var loggerMock = new Mock<ILogger<DefaultSqlSugarDataInterceptor>>();
        _loggerFactoryMock.Setup(x => x.CreateLogger<DefaultSqlSugarDataInterceptor>()).Returns(loggerMock.Object);

        // Act
        _context.OnSqlExecuting(sql, parameters);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("SQL执行")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

}