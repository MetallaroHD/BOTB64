local generic = require("auto_generic")

Log("Pilaf attacks.")
generic.run()

local target = Targets[1]
PlayVfxInstant("slash_h_red", target.Q, target.R)

Success()