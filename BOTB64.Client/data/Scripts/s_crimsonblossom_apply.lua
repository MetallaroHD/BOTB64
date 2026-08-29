-- Crimson Blossom - OnApply: mark active and raise max Bloodwell to 150.

SetAuraParam(Invoker, 17, "Active", 1)
ModifyStat(Invoker, "MaxRes", 50, 0)
Success()
