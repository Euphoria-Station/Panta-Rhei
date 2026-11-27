using Content.Shared._CD.Body;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.NodeContainer;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using YamlDotNet.Core.Tokens;

// This file takes existing user characters that have allergies, and adjusts their data so that they can be loaded to use with the allergy reaction type update.
// This would not be needed if I had the foresight to create a reaction type system (AllergyData) before it was merged to Master.
// Any changes to "AllergyData" will require a migration adjustment so that the new field is added to existing characters.
// Without this file, any characters with the original allergy system will cause the server to hang or crash on startup, unable to parse the data.

[TypeSerializer]
public sealed class AllergyDataSerlializer : ITypeSerializer<AllergyData, MappingDataNode>, ITypeReader<AllergyData, ValueDataNode>
{
    public ValidationNode Validate(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)

    {
        var mapping = new Dictionary<ValidationNode, ValidationNode>();

        foreach (var (key, value) in node.Children)
        {
            var keyNode = node.GetKeyNode(key);

            switch (key)
            {
                case "intensity":
                    mapping.Add(serializationManager.ValidateNode<string>(keyNode, context),
                        serializationManager.ValidateNode<FixedPoint2>(value, context));
                    break;

                case "reactionReagent":
                    mapping.Add(serializationManager.ValidateNode<string>(keyNode, context),
                        serializationManager.ValidateNode<ProtoId<ReagentPrototype>>(value, context));
                    break;
            }
        }

        return new ValidatedMappingNode(mapping);
    }

    public AllergyData Read(ISerializationManager serializationManager,
        MappingDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<AllergyData>? instanceProvider = null)
    {
        var allergy = instanceProvider != null ? instanceProvider() : new AllergyData();

        foreach (var (key, value) in node.Children)
        {
            switch (key)
            {
                case "intensity":
                    allergy.Intensity =
                        serializationManager.Read<FixedPoint2>(value, hookCtx, context);
                    break;

                case "reactionReagent":
                    if (value is ValueDataNode reagentNode && !string.IsNullOrWhiteSpace(reagentNode.Value) &&
                        !string.Equals(reagentNode.Value, "null", StringComparison.OrdinalIgnoreCase))
                    {
                        allergy.ReactionReagent =
                            serializationManager.Read<ProtoId<ReagentPrototype>>(value, hookCtx, context);
                    }

                    break;
            }
        }

        return allergy;
    }

    public ValidationNode Validate(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        ISerializationContext? context = null)
    {
        return serializationManager.ValidateNode<FixedPoint2>(node, context);
    }

    public AllergyData Read(ISerializationManager serializationManager,
        ValueDataNode node,
        IDependencyCollection dependencies,
        SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<AllergyData>? instanceProvider = null)
    {
        var allergy = instanceProvider != null ? instanceProvider() : new AllergyData();

        allergy.Intensity = serializationManager.Read<FixedPoint2>(node, hookCtx, context);
        return allergy;
    }

    public DataNode Write(ISerializationManager serializationManager,
        AllergyData value,
        IDependencyCollection dependencies,
        bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        var mapping = new MappingDataNode();

        mapping.Add("reactionReagent",
            serializationManager.WriteValue(value.ReactionReagent, alwaysWrite, context));

        mapping.Add("intensity",
            serializationManager.WriteValue(value.Intensity, alwaysWrite, context));

        return mapping;
    }
}
