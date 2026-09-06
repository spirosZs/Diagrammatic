namespace Diagrammatic_test.Services
{
    /// <summary>
    /// Where this app reaches the Exercises API.
    /// </summary>
    /// <remarks>
    /// Two addresses, because two different callers reach the API from two different places.
    /// <para>
    /// <see cref="InternalBaseUrl"/> is used by everything that runs inside the Blazor circuit —
    /// the <c>DiagrammaticClient</c> HttpClient and the SignalR hub connections. Those calls leave
    /// the app's own process, so in Docker they go over the compose network (<c>http://web:8080</c>)
    /// and never touch a published port.
    /// </para>
    /// <para>
    /// <see cref="PublicBaseUrl"/> is used for the diagram editor iframe. That URL is rendered into
    /// the page and fetched by the student's browser, so it has to be an address the browser can
    /// resolve — the published port locally, the public hostname once deployed.
    /// </para>
    /// Running with <c>dotnet run</c> both default to the API's published port, so nothing has to be
    /// configured for local development.
    /// </remarks>
    public sealed class ApiOptions
    {
        public const string SectionName = "Api";

        public string InternalBaseUrl { get; set; } = "http://localhost:8083";

        public string PublicBaseUrl { get; set; } = "http://localhost:8083";

        /// <summary>Game hub, negotiated from the server side of the circuit.</summary>
        public string HubUrl => Combine(InternalBaseUrl, "gameHub");

        /// <summary>Diagram editor page, loaded into an iframe by the browser.</summary>
        public string DiagramEditorUrl => Combine(PublicBaseUrl, "diagramEvaluation");

        private static string Combine(string baseUrl, string path) =>
            $"{baseUrl.TrimEnd('/')}/{path}";
    }
}
