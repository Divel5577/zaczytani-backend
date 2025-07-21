using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Zaczytani.Domain.DescriptionEnumConver;

public class DescriptionEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (description != null && description.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null);
            }
        }

        return Enum.Parse<T>(value, true);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var field = typeof(T).GetField(value.ToString());
        var description = field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        writer.WriteStringValue(description);
    }
}