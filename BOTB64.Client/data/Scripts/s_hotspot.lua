-- Hotspot - relocates the Hotspot tile effect toward the targeted tile, up to the
-- remaining movement budget (SpellCastingAction already clamped Targets[1] to that
-- budget, since this spell's TrackedSourceAuraID sources its targeter from the
-- Hotspot's own tracked position/budget rather than Oliver's).

local shockwaveAura = 9
local hotspotTileEffect = 3

local oldQ = GetAuraParam(Invoker, shockwaveAura, "Q")
local oldR = GetAuraParam(Invoker, shockwaveAura, "R")
local dest = Targets[1]

local dist = HexDistance(oldQ, oldR, dest.Q, dest.R)

MoveTileEffect(Invoker, oldQ, oldR, dest.Q, dest.R, hotspotTileEffect, -1)
SetAuraParam(Invoker, shockwaveAura, "Q", dest.Q)
SetAuraParam(Invoker, shockwaveAura, "R", dest.R)

local budget = GetAuraParam(Invoker, shockwaveAura, "Budget")
SetAuraParam(Invoker, shockwaveAura, "Budget", math.max(0, budget - dist))

Success()
