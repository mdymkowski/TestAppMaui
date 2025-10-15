namespace TestAppMaui.Gateway.Application.Common.Interfaces;

public interface IAppDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
