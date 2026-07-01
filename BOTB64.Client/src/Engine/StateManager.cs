using BOTB64.Engine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOTB64.Engine
{
    public static class StateManager
    {
        private static IGameState CurrentState;
        public static IGameState PendingState;

        public static void ChangeState(IGameState newState)
        {
            PendingState = newState;
        }

        public static void FlushPendingState()
        {
            if (PendingState == null)
                return;
            CurrentState?.OnExit();
            CurrentState = PendingState;
            CurrentState.OnEnter();
            PendingState = null;
        }

        public static void Update(float dt) => CurrentState?.Update(dt);
        public static void Render() => CurrentState?.Render();
    }
}
