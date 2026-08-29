-- Flame Pillar - places an invisible trap that explodes on contact.

local flamePillarId = 11
local target = Targets[1]
local dmg = 30 + 1.0 * GetSpellPower(Invoker)

ApplyTileEffect(Invoker, target.Q, target.R, flamePillarId, 6)
SetTileEffectParam(target.Q, target.R, flamePillarId, "OwnerID", Invoker)
SetTileEffectParam(target.Q, target.R, flamePillarId, "Damage", dmg)

Success()
