-- Long Staff - toggle: increases auto-attack range from 2 to 3, but reduces
-- auto-attack base damage while active.

local longstaffId = 25
local rangeBonus = 1
local aaReduction = 0.30

local active = GetAuraParam(Invoker, longstaffId, "Extended")
if active > 0 then
    ModifyStat(Invoker, "AutoAttackRange", -rangeBonus, 0)
    ModifyStat(Invoker, "AutoAttackAP", 0, aaReduction)
    SetAuraParam(Invoker, longstaffId, "Extended", 0)
else
    ModifyStat(Invoker, "AutoAttackRange", rangeBonus, 0)
    ModifyStat(Invoker, "AutoAttackAP", 0, -aaReduction)
    SetAuraParam(Invoker, longstaffId, "Extended", 1)
end

Success()
