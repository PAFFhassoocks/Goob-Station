// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Cojoke <83733158+Cojoke-dot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 pheenty <fedorlukin2006@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Movement.Systems;
using Robust.Shared.Containers; // Goobstation

namespace Content.Shared.Item;

/// <summary>
/// This handles <see cref="HeldSpeedModifierComponent"/>
/// </summary>
public sealed class HeldSpeedModifierSystem : EntitySystem
{
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!; // Goobstation

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<HeldSpeedModifierComponent, GotEquippedHandEvent>(OnGotEquippedHand);
        SubscribeLocalEvent<HeldSpeedModifierComponent, GotUnequippedHandEvent>(OnGotUnequippedHand);
        SubscribeLocalEvent<HeldSpeedModifierComponent, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent>>(OnRefreshMovementSpeedModifiers);

        // Goobstation
        SubscribeLocalEvent<HeldSpeedModifierComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<HeldSpeedModifierComponent, ComponentRemove>(OnComponentRemove);
    }

    private void OnGotEquippedHand(Entity<HeldSpeedModifierComponent> ent, ref GotEquippedHandEvent args)
    {
        _movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
    }

    private void OnGotUnequippedHand(Entity<HeldSpeedModifierComponent> ent, ref GotUnequippedHandEvent args)
    {
        _movementSpeedModifier.RefreshMovementSpeedModifiers(args.User);
    }

    public (float,float) GetHeldMovementSpeedModifiers(EntityUid uid, HeldSpeedModifierComponent component)
    {
        var walkMod = component.WalkModifier;
        var sprintMod = component.SprintModifier;
        if (component.MirrorClothingModifier && TryComp<ClothingSpeedModifierComponent>(uid, out var clothingSpeedModifier))
        {
            walkMod = clothingSpeedModifier.WalkModifier;
            sprintMod = clothingSpeedModifier.SprintModifier;
        }

        return (walkMod, sprintMod);
    }

    private void OnRefreshMovementSpeedModifiers(EntityUid uid, HeldSpeedModifierComponent component, HeldRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
    {
        var (walkMod, sprintMod) = GetHeldMovementSpeedModifiers(uid, component);
        args.Args.ModifySpeed(walkMod, sprintMod);
    }

    // Everything below is Goobstation
    private void OnComponentStartup(Entity<HeldSpeedModifierComponent> ent, ref ComponentStartup args)
    {
        if (_container.TryGetContainingContainer((ent, null, null), out var container))
            _movementSpeedModifier.RefreshMovementSpeedModifiers(container.Owner);
    }

    private void OnComponentRemove(Entity<HeldSpeedModifierComponent> ent, ref ComponentRemove args)
    {
        if (_container.TryGetContainingContainer((ent, null, null), out var container))
            _movementSpeedModifier.RefreshMovementSpeedModifiers(container.Owner);
    }
}
