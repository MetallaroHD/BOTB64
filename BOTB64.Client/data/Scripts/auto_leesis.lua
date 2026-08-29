-- Lee Sis - auto attacks deal 50% bonus damage to targets at exactly range 2.

local attacker = Invoker
local target = Targets[1]
local pos = GetPosition(attacker)

local baseDamage = GetAttackPower(attacker) * GetAutoAttackAP(attacker)
        + GetSpellPower(attacker) * GetAutoAttackSP(attacker)

if HexDistance(pos.Q, pos.R, target.Q, target.R) == 2 then
    baseDamage = baseDamage * 1.5
end

DamageAt(target.Q, target.R, math.floor(baseDamage))
Success()
