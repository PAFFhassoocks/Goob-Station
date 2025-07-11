// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ActiveMammmoth <140334666+ActiveMammmoth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ActiveMammmoth <kmcsmooth@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Misandry <mary@thughunt.ing>
// SPDX-FileCopyrightText: 2025 Solstice <solsticeofthewinter@gmail.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pheenty <fedorlukin2006@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

// using Content.Shared._Goobstation.Boomerang; NO!!
using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Melee.Components;

/// <summary>
/// This is used for a melee weapon that throws whatever gets hit by it in a line
/// until it hits a wall or a time limit is exhausted.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// [Access(typeof(MeleeThrowOnHitSystem), typeof(BoomerangSystem))] // Goobstation Edit - No implicit access
public sealed partial class MeleeThrowOnHitComponent : Component
{
    /// <summary>
    /// The speed at which hit entities should be thrown.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Speed = 10f;

    /// <summary>
    /// The maximum distance the hit entity should be thrown.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Distance = 20f;

    /// <summary>
    /// Whether or not anchorable entities should be unanchored when hit.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool UnanchorOnHit;

    /// <summary>
    /// How long should this stun the target, if applicable?
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan? StunTime;

    /// <summary>
    /// Should this also work on a throw-hit?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ActivateOnThrown;

    /// <summary>
    /// Goobstation - should it throw while being on delay?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ThrowWhileOnDelay;
}

/// <summary>
/// Raised a weapon entity with <see cref="MeleeThrowOnHitComponent"/> to see if a throw is allowed.
/// </summary>
[ByRefEvent]
public record struct AttemptMeleeThrowOnHitEvent(EntityUid Target, EntityUid? User, bool Cancelled = false, bool Handled = false);

/// <summary>
/// Raised a target entity before it is thrown by <see cref="MeleeThrowOnHitComponent"/>.
/// </summary>
[ByRefEvent]
public record struct MeleeThrowOnHitStartEvent(EntityUid Weapon, EntityUid? User);
