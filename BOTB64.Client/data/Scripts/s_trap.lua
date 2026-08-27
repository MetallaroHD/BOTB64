-- Trap - places a hidden trap (TileEffect ID 1) that roots enemies on contact

local target = Targets[1]

ApplyTileEffect(Invoker, target.Q, target.R, 1, 4)
Success()
