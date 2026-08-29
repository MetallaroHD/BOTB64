-- Invoke the Warrior Gods - immediately fills Rage and grants a Speed buff.

local invokeBuffId = 32

ModifyResource(Invoker, 100)
ApplyAura(Invoker, Invoker, invokeBuffId, 1)

Success()
