using System.Text.Json.Nodes;

using EnumsNET;

using WhiteEstate.DocFormat.Json.Serializers;
namespace WhiteEstate.DocFormat.Json.Internal;

internal static class SerializationExtensions
{
    private static string AsString(this JsonNodeType nodeType)
    {
        return nodeType.AsString(EnumFormat.Description, EnumFormat.Name)!;
    }

    public static JsonNodeType? GetJsonNodeType(this JsonObject nodeType)
    {
        return EnumsNET.Enums.TryParse((string)nodeType[Fields.Kind]!, true, out JsonNodeType result, EnumFormat.Description,
            EnumFormat.Name)
            ? result
            : null;
    }

    public static JsonObject WithJsonNodeType(this JsonObject jsonObject, JsonNodeType nodeType)
    {
        jsonObject[Fields.Kind] = nodeType.AsString();
        return jsonObject;
    }

    public static JsonObject WithAttribute(this JsonObject element, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            element[name] = value;
        }

        return element;
    }

    // public static JsonObject WithFloatAttribute(this JsonObject element, string name, float value)
    // {
    //     element[name] = value;
    //     return element;
    // }
    //
    // public static JsonObject WithFloatAttribute(this JsonObject element, string name, float? value)
    // {
    //     return value is null ? element : WithFloatAttribute(element, name, value.Value);
    // }
    public static JsonObject WithIntAttribute(this JsonObject element, string name, int value)
    {
        element[name] = value;
        return element;
    }

    public static JsonObject WithIntAttribute(this JsonObject element, string name, int? value)
    {
        return value is null ? element : WithIntAttribute(element, name, value.Value);
    }

    public static JsonObject WithBooleanAttribute(this JsonObject element, string name, bool value)
    {
        if (value)
        {
            element[name] = value;
        }

        return element;
    }

    public static JsonObject WithSubtype<T>(this JsonObject element, T value) where T : struct, Enum
    {
        return WithEnumAttribute(element, Fields.Type, value);
    }

    public static JsonObject WithChild(this JsonObject element, JsonObject value)
    {
        element["child"] = value;
        return element;
    }

    public static JsonObject WithEnumAttribute<T>(this JsonObject element, string name, T? value) where T : struct, Enum
    {
        if (value is not null)
        {
            WithEnumAttribute(element, name, value.Value);
        }

        return element;
    }

    public static JsonObject WithEnumAttribute<T>(this JsonObject element, string name, T value) where T : struct, Enum
    {
        element[name] = value.AsString(EnumFormat.Description);
        return element;
    }

    public static JsonObject WithChildren(this JsonObject element,
        IWemlJsonSerializer serializer,
        IEnumerable<IWemlNode> nodes,
        string key = "children")
    {
        var array = new JsonArray();
        foreach (JsonObject node in nodes.Select(serializer.Serialize))
        {
            array.Add(node);
        }

        element[key] = array;
        return element;
    }

    public static IEnumerable<IWemlNode> GetChildren(this JsonObject element,
        IWemlJsonDeserializer deserializer,
        string key = Fields.Children)
    {
        if (!element.ContainsKey(key) || element[key] is not JsonArray children)
        {
            yield break;
        }


        foreach (JsonNode? child in children)
        {
            if (child is JsonObject childObject)
            {
                IWemlNode childNode = deserializer.Deserialize(childObject);
                yield return childNode;
            }
            else
            {
                throw new JsonDeserializationException("Array node is not an object", element);
            }
        }
    }

    /// <summary>
    /// Ensures that WEML nodes are of required type
    /// </summary>
    /// <param name="nodes">List of nodes</param>
    /// <param name="parent">Parent node</param>
    /// <typeparam name="T">Needed type</typeparam>
    /// <returns></returns>
    public static T[] EnsureNodeTypes<T>(this IEnumerable<IWemlNode?> nodes, JsonObject parent) where T : IWemlNode
    {
        var result = new List<T>();
        bool requiresBlock = typeof(T).IsAssignableTo(typeof(IWemlBlockNode));
        foreach (IWemlNode? node in nodes)
        {
            if (node is not T value)
            {
                if (requiresBlock && node is WemlTextNode text && string.IsNullOrWhiteSpace(text.Text))
                {
                    continue;
                }

                throw new JsonDeserializationException($"Node {node?.GetType().ToString() ?? "`null`"} is not {typeof(T)}", parent);
            }

            result.Add(value);
        }

        return result.ToArray();
    }
}