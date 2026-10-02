using System.Text.Json;
using System.Text.Json.Serialization;
using MiniCrm.Application.Common;

namespace MiniCrm.Api.Serialization;

public class ArgentinaDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var dateTime))
        {
            return dateTime;
        }

        return DateTime.Parse(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var local = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
        var offset = new DateTimeOffset(local, ArgentinaTime.OffsetFor(local));
        writer.WriteStringValue(offset.ToString("yyyy-MM-dd'T'HH:mm:sszzz"));
    }
}

public class ArgentinaNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly ArgentinaDateTimeConverter Inner = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return Inner.Read(ref reader, typeof(DateTime), options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        Inner.Write(writer, value.Value, options);
    }
}
