// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Holiday;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Client.Holiday;

public sealed class HolidaySystem : EntitySystem
{
    [Dependency] private readonly IResourceCache _rescache = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<HolidayRsiSwapComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(Entity<HolidayRsiSwapComponent> ent, ref AppearanceChangeEvent args)
    {
        if (!_appearance.TryGetData<string>(ent, HolidayVisuals.Holiday, out var data, args.Component))
            return;

        var comp = ent.Comp;
        if (!comp.Sprite.TryGetValue(data, out var rsistring) || args.Sprite == null)
            return;

        var path = SpriteSpecifierSerializer.TextureRoot / rsistring;
        if (_rescache.TryGetResource(path, out RSIResource? rsi))
            _sprite.SetBaseRsi((ent.Owner, args.Sprite), rsi.RSI);
    }
}