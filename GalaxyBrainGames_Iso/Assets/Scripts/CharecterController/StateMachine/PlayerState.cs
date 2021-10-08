using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.States
{
    public abstract class PlayerState
    {
        protected PlayerController controller;
        protected PlayerStateMachine machine;

        public PlayerStateMachine Machine
        {
            get { return machine; }
            set { machine = value; }
        }

        public PlayerState(PlayerController controller)
        {
            this.controller = controller;
        }

        public virtual void OnStateStart() { }
        public virtual void OnStateEnd() { }
        public virtual void OnStateUpdate() { }
    }
}
