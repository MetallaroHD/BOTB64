-- Scoper ondeath - apply/drop
-- ID 2

local applydrop = require("a_applydrop")

if applydrop.run(Invoker, "AttackDamage", 0, 2) then
    Success()
else
    Fail("Bad stat name!")
end