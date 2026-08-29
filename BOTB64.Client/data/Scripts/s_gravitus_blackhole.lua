-- Black Hole - creates a black hole that pulls enemies for 3 turns; touching it
-- triggers an explosion.

local blackHoleId = 13
local target = Targets[1]
local dmg = 30 + 1.0 * GetSpellPower(Invoker)

ApplyTileEffect(Invoker, target.Q, target.R, blackHoleId, 3)
SetTileEffectParam(target.Q, target.R, blackHoleId, "OwnerID", Invoker)
SetTileEffectParam(target.Q, target.R, blackHoleId, "Damage", dmg)

Success()
