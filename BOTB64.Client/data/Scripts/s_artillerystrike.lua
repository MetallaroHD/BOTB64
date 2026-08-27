-- Artillery Strike - starts charging; the actual explosion is handled by
-- s_artillerystrike_explode.lua, run OnStartTurn by the "Artillery Charging" aura
-- this applies (mirrors how Snipe's delayed shot works via the "Taking Aim" aura).

local chargingAura = 10
ApplyAura(Invoker, Invoker, chargingAura, 1)
Success()
