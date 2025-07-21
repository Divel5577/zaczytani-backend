using System.ComponentModel;
using System.Reflection;

namespace Zaczytani.Domain.Helpers;

public static class EnumHelper
{
    public static List<string> GetEnumDescriptions<T>() where T : Enum
    {
        return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Select(field => field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? field.Name)
                        .ToList();
    }

    public static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static T GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if ((attribute != null && attribute.Description == description) || field.Name == description)
            {
                return (T)field.GetValue(null);
            }
        }

        throw new ArgumentException($"No enum value found for description '{description}' in {typeof(T).Name}.");
    }
}