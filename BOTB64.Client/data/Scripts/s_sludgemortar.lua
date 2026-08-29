-- Sludge Mortar - shoots dealing Spell Power damage and creating a Venom Trail tile

local venomTrailId = 7
local pos = Targets[1]
local target = GetCharacterAt(pos.Q, pos.R)

if target >= 0 and IsEnemy(Invoker, target) then
    Damage(target, 25 + 1.1 * GetSpellPower(Invoker))
end

ApplyTileEffect(Invoker, pos.Q, pos.R, venomTrailId, 25)
SetTileEffectParam(pos.Q, pos.R, venomTrailId, "OwnerID", Invoker)

Success()
