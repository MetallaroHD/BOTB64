-- Bloodthirst - spends Rage to deal melee damage and apply a heal over time to
-- himself.

local hotId = 31
local target = GetCharacterAt(Targets[1].Q, Targets[1].R)
local dmg = 25 + 1.0 * GetAttackPower(Invoker)

if target >= 0 and IsEnemy(Invoker, target) then
    Damage(target, math.floor(dmg))
end

ApplyAura(Invoker, Invoker, hotId, 1)

Success()
