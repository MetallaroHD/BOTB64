-- Disengage - Scoper quickly leaps to target location

local target = Targets[1]

if not ForceMove(Invoker, target.Q, target.R) then
    Fail("Can't leap there!")
    return
end

Success()
