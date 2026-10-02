namespace MiniCrm.Application.Exceptions;

public class BusinessValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message)
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}
