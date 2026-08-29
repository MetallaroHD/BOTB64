-- Bloodlink - while Trappus has Crimson Blossom active, the linked ally deals and
-- takes 50% more damage (this script is used for both OnPreDamageDealt, when the ally
-- is attacking, and OnPreDamageTaken, when the ally is defending).

local bloodlinkId = 16
local crimsonId = 17
local trappusId = GetAuraParam(Invoker, bloodlinkId, "TrappusID")

if Invoker ~= trappusId and GetAuraParam(trappusId, crimsonId, "Active") > 0 then
    ScaleDamage(1.5)
end

Success()
