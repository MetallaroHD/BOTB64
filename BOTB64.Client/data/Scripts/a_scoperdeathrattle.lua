-- Scoper deathrattle - ondeath
-- ID 6

local myPos = GetPosition(Invoker)

if myPos.Q ~= -999 and myPos.R ~= -999 then
    ApplyTileEffect(Invoker, myPos.Q, myPos.R, 1, 5)
end
Success()