-- Bloodthirst - heals Pilaf at the start of each of his turns.

local healAmount = 15 + 0.15 * GetAttackPower(Invoker)
Heal(Invoker, math.floor(healAmount))

Success()
