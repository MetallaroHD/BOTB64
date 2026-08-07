-- lens clean buff
--

local applydrop = require("a_applydrop")

if applydrop.run(Invoker, "AttackPower", 15, 0) then
    Success()
else
    Fail("Bad stat name!")
end