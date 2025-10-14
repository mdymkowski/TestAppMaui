namespace TestAppMaui.Application.Common.Interfaces;

public interface IAppDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
