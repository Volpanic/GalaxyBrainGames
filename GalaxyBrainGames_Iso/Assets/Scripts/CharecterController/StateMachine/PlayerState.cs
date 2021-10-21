namespace GalaxyBrain.Creatures.States
{
    /// <summary>
    /// Base class for different states in the players state machine
    /// </summary>
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
