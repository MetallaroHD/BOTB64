-- Shockwave (passive) - places the Hotspot at (0,0) the first time this runs, and
-- refreshes Oliver's remaining Hotspot-movement budget every turn thereafter.
-- Aura ID 9 (Shockwave, this aura) is also used by SpellCastingAction as the
-- "TrackedSourceAuraID" for the Hotspot spell - it reads this aura's "Q"/"R"/"Budget"
-- params to know where the Hotspot currently is and how far it can still be moved.

local shockwaveAura = 9
local hotspotTileEffect = 3

local placed = GetAuraParam(Invoker, shockwaveAura, "Placed")
if placed == 0 then
    ApplyTileEffect(Invoker, 0, 0, hotspotTileEffect, -1)
    SetAuraParam(Invoker, shockwaveAura, "Q", 0)
    SetAuraParam(Invoker, shockwaveAura, "R", 0)
    SetAuraParam(Invoker, shockwaveAura, "Placed", 1)
end

SetAuraParam(Invoker, shockwaveAura, "Budget", GetSpeed(Invoker))
Success()
