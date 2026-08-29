-- Bloodwell (passive) - on death, creates an unbreakable Blood Blossom statue at the
-- spot, remembering how much Bloodwell Trappus had so the blossom can heal by that much.
-- (Its OnRoundStart trigger already sees Invoker = the blossom's Owner, i.e. Trappus,
-- via ApplyTileEffect's Owner field - no need to separately track that here.)

local bloodBlossomId = 6
local pos = GetPosition(Invoker)
local bloodwell = GetResource(Invoker)

if pos.Q ~= -999 and pos.R ~= -999 then
    ApplyTileEffect(Invoker, pos.Q, pos.R, bloodBlossomId, 3)
    SetTileEffectParam(pos.Q, pos.R, bloodBlossomId, "HealAmount", bloodwell)
end

Success()
