-- Scoper reload aura - shared by both the OnApply and OnDrop Effect entries
-- ID 1

if HasTrigger(EffectTrigger.OnApply) then
    local rng = Random(0, 0.25)
    if not ModifyStat(Invoker, "AutoAttackAP", 0, rng) then
        Fail("Bad stat name!")
        return
    end
    SetAuraParam(Invoker, 1, "Snapshot", rng)
else
    local snap = GetAuraParam(Invoker, 1, "Snapshot")
    if not ModifyStat(Invoker, "AutoAttackAP", 0, -snap) then
        Fail("Bad stat name!")
        return
    end
    SetAuraParam(Invoker, 1, "Snapshot", 0)
end
Success()