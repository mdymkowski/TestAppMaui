using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TestAppMaui.Infrastructure.Options;

public sealed class CrmOptions
{
    public string? BaseUrl { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Domain { get; set; }

    public string TaskEntitySetName { get; set; } = "tasks";

    public string IdAttribute { get; set; } = "activityid";

    public string TitleAttribute { get; set; } = "subject";

    public string DescriptionAttribute { get; set; } = "description";

    public string DueDateAttribute { get; set; } = "scheduledend";

    public string StateAttribute { get; set; } = "statecode";

    public int CompletedStateValue { get; set; } = 1;

    public int ActiveStateValue { get; set; } = 0;

    internal bool IsConfigured => !string.IsNullOrWhiteSpace(BaseUrl);

    internal void CopyTo(CrmOptions target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.BaseUrl = BaseUrl;
        target.Username = Username;
        target.Password = Password;
        target.Domain = Domain;
        target.TaskEntitySetName = TaskEntitySetName;
        target.IdAttribute = IdAttribute;
        target.TitleAttribute = TitleAttribute;
        target.DescriptionAttribute = DescriptionAttribute;
        target.DueDateAttribute = DueDateAttribute;
        target.StateAttribute = StateAttribute;
        target.CompletedStateValue = CompletedStateValue;
        target.ActiveStateValue = ActiveStateValue;
    }

    internal void ConfigureClient(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("CRM BaseUrl must be configured before using the CRM service.");
        }

        client.BaseAddress = new Uri(BaseUrl, UriKind.Absolute);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        client.DefaultRequestHeaders.Add("Prefer", "return=representation");
    }

    internal HttpMessageHandler CreateHandler()
    {
        var handler = new HttpClientHandler
        {
            PreAuthenticate = true
        };

        if (!string.IsNullOrWhiteSpace(Username))
        {
            handler.Credentials = string.IsNullOrWhiteSpace(Domain)
                ? new NetworkCredential(Username, Password)
                : new NetworkCredential(Username, Password, Domain);
        }

        return handler;
    }
}
