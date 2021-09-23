using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Creatures.States
{
    public class PlayerStateMachine
    {
        private Dictionary<Type, PlayerState> stateLookup;
        private PlayerState defaultState;

        private PlayerState currentState;
        
        private bool lockedState = false;

        public PlayerState CurrentState
        {
            get { return currentState; }
        }

        public bool InDefaultState
        {
            get { return currentState == defaultState; }
        }

        public bool LockState
        {
            get { return lockedState ; }
            set { lockedState = value; }
        }

        public PlayerStateMachine(PlayerState defaultState)
        {
            stateLookup = new Dictionary<Type, PlayerState>();
            this.defaultState = defaultState;

            if (defaultState != null)
            {
                stateLookup.Add(defaultState.GetType(),defaultState);
                defaultState.OnStateStart();
                defaultState.Machine = this;

                currentState = defaultState;
            }
        }

        public void UpdateState()
        {
            currentState?.OnStateUpdate();
        }

        public void AddState(PlayerState state)
        {
            state.Machine = this;
            stateLookup.Add(state.GetType(), state);
        }

        public void ChangeState(Type stateType)
        {
            if (lockedState) return;

            PlayerState targetState = stateLookup.ContainsKey(stateType) ? stateLookup[stateType] : null;
            currentState?.OnStateEnd();
            targetState?.OnStateStart();

            currentState = targetState;
        }

        public void ChangeToDefaultState()
        {
            if (lockedState) return;

            currentState?.OnStateEnd();
            defaultState?.OnStateStart();

            currentState = defaultState;
        }
    }
}
