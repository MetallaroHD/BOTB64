-- Clean Scope - a target ally spends their fast action to clean Scoper's lens,
-- increasing Scoper's Attack Power (via the "Clean Scope" aura, ID 4)

local target = GetCharacterAt(Targets[1].Q, Targets[1].R)

if target < 0 then
    Fail("No target!")
    return
end

SpendAction(target, true)
ApplyAura(Invoker, Invoker, 4, 1)
ApplyAura(Invoker, target, 5, 1)
Success()
