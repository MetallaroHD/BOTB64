-- Crimson Blossom - OnDrop: revert max Bloodwell, clamping current Bloodwell down if
-- it was sitting above the normal 100 cap.

SetAuraParam(Invoker, 17, "Active", 0)
ModifyStat(Invoker, "MaxRes", -50, 0)

local current = GetResource(Invoker)
if current > 100 then
    ModifyResource(Invoker, 100 - current)
end

Success()
