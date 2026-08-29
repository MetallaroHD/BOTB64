-- Long Staff - upon death, plants an indestructible statue at Lee Sis's position that
-- keeps attacking nearby enemies every round.

local statueId = 14
local pos = GetPosition(Invoker)
local dmg = 15 + 0.6 * GetAttackPower(Invoker)

ApplyTileEffect(Invoker, pos.Q, pos.R, statueId, -1)
SetTileEffectParam(pos.Q, pos.R, statueId, "OwnerID", Invoker)
SetTileEffectParam(pos.Q, pos.R, statueId, "Damage", dmg)

Success()
