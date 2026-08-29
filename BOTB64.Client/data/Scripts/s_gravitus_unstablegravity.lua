-- Unstable Gravity - a temporary (3-turn) tether between two independently targeted
-- entities, working just like Gravity Tether but tracked separately.

local supermassiveId = 34
local unstableTetherId = 37

local posA = Targets[1]
local posB = Targets[2]
local a = GetCharacterAt(posA.Q, posA.R)
local b = GetCharacterAt(posB.Q, posB.R)

if a < 0 or b < 0 or a == b then
    Fail("Must target two different characters!")
    return
end

local hasPrev = GetAuraParam(Invoker, supermassiveId, "HasUnstableTether")
if hasPrev > 0 then
    local prevA = GetAuraParam(Invoker, supermassiveId, "UnstableTetherA")
    local prevB = GetAuraParam(Invoker, supermassiveId, "UnstableTetherB")
    DropAura(prevA, unstableTetherId, 1)
    DropAura(prevB, unstableTetherId, 1)
end

ApplyAura(Invoker, a, unstableTetherId, 1)
ApplyAura(Invoker, b, unstableTetherId, 1)
SetAuraParam(a, unstableTetherId, "PartnerID", b)
SetAuraParam(a, unstableTetherId, "GravitusID", Invoker)
SetAuraParam(b, unstableTetherId, "PartnerID", a)
SetAuraParam(b, unstableTetherId, "GravitusID", Invoker)

SetAuraParam(Invoker, supermassiveId, "UnstableTetherA", a)
SetAuraParam(Invoker, supermassiveId, "UnstableTetherB", b)
SetAuraParam(Invoker, supermassiveId, "HasUnstableTether", 1)

Success()
