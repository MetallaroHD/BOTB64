-- Bloodwell (passive) - autoattacks generate 5 Bloodwell, tripled while Crimson
-- Blossom is active.

local crimsonId = 17
local gain = 5

if GetAuraParam(Invoker, crimsonId, "Active") > 0 then
    gain = gain * 3
end

ModifyResource(Invoker, gain)
Success()
