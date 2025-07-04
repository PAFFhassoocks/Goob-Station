// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 SX-7 <sn1.test.preria.2002@gmail.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 gluesniffler <linebarrelerenthusiast@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Emp;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Cybernetics;
using Content.Shared.Body.Part;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._Shitmed.Cybernetics;

internal sealed class CyberneticsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedBodySystem _body = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<CyberneticsComponent, EmpPulseEvent>(OnEmpPulse);
        SubscribeLocalEvent<CyberneticsComponent, EmpDisabledRemoved>(OnEmpDisabledRemoved);
    }
    private void OnEmpPulse(Entity<CyberneticsComponent> cyberEnt, ref EmpPulseEvent ev)
    {
        if (!cyberEnt.Comp.Disabled)
        {
            ev.Affected = true;
            ev.Disabled = true;
            cyberEnt.Comp.Disabled = true;

            if (HasComp<OrganComponent>(cyberEnt))
            {
                var disableEvent = new OrganEnableChangedEvent(false);
                RaiseLocalEvent(cyberEnt, ref disableEvent);
            }
            else if (TryComp(cyberEnt, out BodyPartComponent? part))
            {
                var disableEvent = new BodyPartEnableChangedEvent(false);
                RaiseLocalEvent(cyberEnt, ref disableEvent);

                if (TryComp(cyberEnt, out DamageableComponent? damageable)
                    && part.Body is not null)
                {
                    var shock = new DamageSpecifier(_prototypes.Index<DamageTypePrototype>("Shock"), 30);
                    var targetPart = _body.GetTargetBodyPart(part);
                    _damageable.TryChangeDamage(part.Body.Value, shock, ignoreResistances: true, targetPart: targetPart, damageable: damageable);
                    Dirty(cyberEnt, damageable);
                }
            }
        }
    }

    private void OnEmpDisabledRemoved(Entity<CyberneticsComponent> cyberEnt, ref EmpDisabledRemoved ev)
    {
        if (cyberEnt.Comp.Disabled)
        {
            cyberEnt.Comp.Disabled = false;
            if (HasComp<OrganComponent>(cyberEnt))
            {
                var enableEvent = new OrganEnableChangedEvent(true);
                RaiseLocalEvent(cyberEnt, ref enableEvent);
            }
            else if (HasComp<BodyPartComponent>(cyberEnt))
            {
                var enableEvent = new BodyPartEnableChangedEvent(true);
                RaiseLocalEvent(cyberEnt, ref enableEvent);
            }
        }
    }
}
