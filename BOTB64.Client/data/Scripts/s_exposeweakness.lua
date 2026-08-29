-- Expose Weakness - sets an enemy's Defense and Magic Defense to 0 for 2 turns

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 or not IsEnemy(Invoker, target) then
    Fail("No valid target!")
    return
end

ApplyAura(Invoker, target, 12, 1)
Success()
