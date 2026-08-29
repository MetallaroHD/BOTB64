-- Storm Shield - every time Lei Stess takes damage, she gains 2 more Thunder Ritual
-- stacks and strikes the attacker back.

local auraId = 11
local stacks = GetAuraParam(Invoker, auraId, "Stacks") + 2
SetAuraParam(Invoker, auraId, "Stacks", stacks)
ModifyStat(Invoker, "SpellPower", 8, 0)

if Attacker ~= nil and IsAlive(Attacker) then
    Damage(Attacker, 20 + 0.3 * GetSpellPower(Invoker))
end

Success()
