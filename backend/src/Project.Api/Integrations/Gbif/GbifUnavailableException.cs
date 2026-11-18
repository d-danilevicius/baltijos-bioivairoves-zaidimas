namespace Project.Api.Integrations.Gbif;

// GBIF did not answer in time, returned an error or returned data that cannot be read (NFR-020)
public class GbifUnavailableException : Exception
{
    public GbifUnavailableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
