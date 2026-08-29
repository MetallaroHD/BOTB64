-- Meditate - buffs Attack Power and heals a bit.

local meditateBuffId = 27
local healAmount = 40 + 0.3 * GetAttackPower(Invoker)

ApplyAura(Invoker, Invoker, meditateBuffId, 1)
Heal(Invoker, math.floor(healAmount))

Success()
