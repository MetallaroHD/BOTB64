-- Gravitational Pull - pulls the two Gravity Tether targets toward each other,
-- stopping each at walls. Slows enemies of Gravitus, hastens his allies. Fails if no
-- Gravity Tether is currently active.

local supermassiveId = 34
local pullSlowId = 35
local pullHasteId = 36

local hasTether = GetAuraParam(Invoker, supermassiveId, "HasTether")
if hasTether <= 0 then
    Fail("No Gravity Tether active!")
    return
end

local a = GetAuraParam(Invoker, supermassiveId, "TetherA")
local b = GetAuraParam(Invoker, supermassiveId, "TetherB")

if not IsAlive(a) or not IsAlive(b) then
    Fail("No Gravity Tether active!")
    return
end

local posA = GetPosition(a)
local posB = GetPosition(b)
local dist = HexDistance(posA.Q, posA.R, posB.Q, posB.R)
local pullEach = math.floor(dist / 2)

if pullEach > 0 then
    PullToward(a, posB.Q, posB.R, pullEach)
    PullToward(b, posA.Q, posA.R, pullEach)
end

if IsEnemy(Invoker, a) then
    ApplyAura(Invoker, a, pullSlowId, 1)
else
    ApplyAura(Invoker, a, pullHasteId, 1)
end

if IsEnemy(Invoker, b) then
    ApplyAura(Invoker, b, pullSlowId, 1)
else
    ApplyAura(Invoker, b, pullHasteId, 1)
end

Success()
