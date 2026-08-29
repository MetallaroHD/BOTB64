-- Forbidden Ritual - spends half of current Bloodwell, dealing that much damage to
-- an enemy and healing Trappus for half of what was spent.

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or not IsEnemy(Invoker, target) then
    Fail("No valid target!")
    return
end

local bloodwell = GetResource(Invoker)
if bloodwell <= 0 then
    Fail("Not enough Bloodwell!")
    return
end

local spent = math.floor(bloodwell / 2)
ModifyResource(Invoker, -spent)
Damage(target, spent)
Heal(Invoker, math.floor(spent / 2))

Success()
