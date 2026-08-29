-- Bloodlink - when the linked ally deals damage, Trappus generates Bloodwell equal
-- to half of it (tripled while Trappus has Crimson Blossom active). Does nothing when
-- Trappus himself is the one dealing the damage.

local bloodlinkId = 16
local crimsonId = 17
local trappusId = GetAuraParam(Invoker, bloodlinkId, "TrappusID")

if Invoker ~= trappusId then
    local gain = math.floor(DamageAmount / 2)
    if GetAuraParam(trappusId, crimsonId, "Active") > 0 then
        gain = gain * 3
    end
    ModifyResource(trappusId, gain)
end

Success()
