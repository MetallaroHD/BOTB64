-- Revitalize - damages Trappus by 50% of his current Bloodwell (without spending it)
-- and heals a target ally by the same amount.

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or IsEnemy(Invoker, target) then
    Fail("Must target an ally!")
    return
end

local bloodwell = GetResource(Invoker)
local amount = math.floor(bloodwell / 2)

if amount <= 0 then
    Fail("Not enough Bloodwell!")
    return
end

Damage(Invoker, amount)
Heal(target, amount)

Success()
