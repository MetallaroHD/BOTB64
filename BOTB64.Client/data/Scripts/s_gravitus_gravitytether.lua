-- Gravity Tether - links two independently targeted entities. Permanent; recasting
-- on two new targets removes the previous pair first.

local supermassiveId = 34
local tetherId = 33

local posA = Targets[1]
local posB = Targets[2]
local a = GetCharacterAt(posA.Q, posA.R)
local b = GetCharacterAt(posB.Q, posB.R)

if a < 0 or b < 0 or a == b then
    Fail("Must target two different characters!")
    return
end

local hasPrev = GetAuraParam(Invoker, supermassiveId, "HasTether")
if hasPrev > 0 then
    local prevA = GetAuraParam(Invoker, supermassiveId, "TetherA")
    local prevB = GetAuraParam(Invoker, supermassiveId, "TetherB")
    DropAura(prevA, tetherId, 1)
    DropAura(prevB, tetherId, 1)
end

ApplyAura(Invoker, a, tetherId, 1)
ApplyAura(Invoker, b, tetherId, 1)
SetAuraParam(a, tetherId, "PartnerID", b)
SetAuraParam(a, tetherId, "GravitusID", Invoker)
SetAuraParam(b, tetherId, "PartnerID", a)
SetAuraParam(b, tetherId, "GravitusID", Invoker)

SetAuraParam(Invoker, supermassiveId, "TetherA", a)
SetAuraParam(Invoker, supermassiveId, "TetherB", b)
SetAuraParam(Invoker, supermassiveId, "HasTether", 1)

Success()
