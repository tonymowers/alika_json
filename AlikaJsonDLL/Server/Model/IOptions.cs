namespace CH.Alika.Json.Server.Model
{
    public interface IOptions
    {
        bool IsArray { get; }
        bool IsArrayOfArray { get;  }
        bool IsFormatted { get; }
    }
}