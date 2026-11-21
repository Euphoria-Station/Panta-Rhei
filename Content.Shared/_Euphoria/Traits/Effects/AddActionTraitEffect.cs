using Content.Shared._DV.Traits.Effects;
using Content.Shared.Actions;
using Content.Shared.Mind;
using Robust.Shared.Prototypes;

namespace Content.Shared._Euphoria.Traits.Effects;

public sealed partial class AddActionTraitEffect : BaseTraitEffect
{
    ActionContainerSystem actsys = default!;
    [DataField(required: true)]
    public EntProtoId Action;

    public override void Apply(TraitEffectContext ctx)
    {
        try
        {
            SharedMindSystem mindsys = ctx.EntMan.System<SharedMindSystem>();
            ActionContainerSystem actsys = ctx.EntMan.System<ActionContainerSystem>();
            if (!mindsys.TryGetMind(ctx.Player, out var mind, out _))
                return;
            actsys.AddAction(mind, Action);

        }
        catch (Exception e)
        {
            Log.Error($"Exception while trying to add action: {e}");
        }
    }
}
