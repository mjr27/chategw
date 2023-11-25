using System.Text.Json.Nodes;

using WhiteEstate.DocFormat.Json.Serializers.Block;
using WhiteEstate.DocFormat.Json.Serializers.BlockChildren;
using WhiteEstate.DocFormat.Json.Serializers.Container;
using WhiteEstate.DocFormat.Json.Serializers.Inline;
namespace WhiteEstate.DocFormat.Json.Internal;

/// <summary>
/// Serializer repository
/// </summary>
internal class WemlSerializerRepository
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public WemlSerializerRepository()
    {
        Register(new TextNodeSerializer());

        Register(new AnchorSerializer());
        Register(new LanguageSerializer());
        Register(new EntitySerializer());
        Register(new FormatSerializer());
        Register(new LineBreakSerializer());
        Register(new LinkSerializer());
        Register(new NonEgwSerializer());
        Register(new NoteSerializer());
        Register(new PageBreakSerializer());
        Register(new SentenceSerializer());

        Register(new FigureSerializer());
        Register(new TableSerializer());
        Register(new TableCellSerializer());
        Register(new TableRowSerializer());

        Register(new ListSerializer());
        Register(new ListItemSerializer());

        Register(new ParagraphSerializer());
        Register(new ThoughtBreakSerializer());


        Register(new ParagraphContainerSerializer());
        Register(new ParagraphGroupContainerSerializer());
        Register(new HeadingContainerSerializer());
    }

    private readonly Dictionary<Type, Func<IWemlJsonSerializer, IWemlNode, JsonObject>>
        _serializers = new();

    private readonly List<Func<IWemlJsonDeserializer, JsonObject, IWemlNode?>>
        _deserializers = new();

    /// <summary>
    /// Registers a serializer
    /// </summary>
    /// <param name="serializer"></param>
    /// <typeparam name="T"></typeparam>
    private void Register<T>(IWemlJsonElementSerializer<T> serializer) where T : IWemlNode
    {
        _serializers[typeof(T)] = (ser, root) => serializer.Serialize(
            ser,
            (T)root
        );
        _deserializers.Add((deserializer, node) => serializer.Deserialize(
            deserializer,
            node
        ));
    }

    /// <summary>
    /// Serializes
    /// </summary>
    /// <param name="root"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    /// <exception cref="JsonSerializationException"></exception>
    public JsonObject Serialize(IWemlNode root, IWemlJsonSerializer serializer) =>
        _serializers.GetValueOrDefault(root.GetType())?.Invoke(serializer, root)
        ?? throw new JsonSerializationException("Invalid node type", root.GetType());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="root"></param>
    /// <param name="deserializer"></param>
    /// <exception cref="JsonDeserializationException"></exception>
    /// <returns></returns>
    public IWemlNode Deserialize(JsonObject root, IWemlJsonDeserializer deserializer) =>
        _deserializers
            .Select(r => r(deserializer, root))
            .FirstOrDefault(resultingNode => resultingNode is not null)
        ?? throw new JsonDeserializationException($"Cannot find deserializer for {root.ToJsonString()}", root);
}