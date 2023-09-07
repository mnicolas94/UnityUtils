using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

namespace Utils.Editor.AttributeDrawers.Animations
{
    public static class AnimatorControllerExtensions
    {
        public static List<AnimatorState> GetStates(this AnimatorController controller)
        {
            int layersCount = controller.layers.Length;
            var animatorStates = new List<AnimatorState>(layersCount);
            
            var stateMachines = controller.layers.Select(layer => layer.stateMachine).ToList();

            while (stateMachines.Count > 0)
            {
                var stateMachine = stateMachines[0];
                stateMachines.RemoveAt(0);
                
                // add children state machines
                var subMachines = stateMachine.stateMachines.Select(sm => sm.stateMachine);
                stateMachines.AddRange(subMachines);
                
                // add states
                var states = stateMachine.states.Select(s => s.state);
                foreach (var state in states)
                {
                    var alreadyAdded = animatorStates.Exists(s => s.nameHash == state.nameHash);
                    if (!alreadyAdded)
                    {
                        animatorStates.Add(state);
                    }
                }
            }

            return animatorStates;
        }
        
        public static List<AnimatorTransition> GetTransitions(this AnimatorController controller)
        {
            int layersCount = controller.layers.Length;
            var animatorTransitions = new List<AnimatorTransition>(layersCount);
            
            var stateMachines = controller.layers.Select(layer => layer.stateMachine).ToList();

            while (stateMachines.Count > 0)
            {
                var stateMachine = stateMachines[0];
                stateMachines.RemoveAt(0);
                
                // add children state machines
                var subMachines = stateMachine.stateMachines.Select(sm => sm.stateMachine);
                stateMachines.AddRange(subMachines);
                
                // add states
                var states = stateMachine.states.Select(s => s.state);
                foreach (var state in states)
                {
                    var transitions = stateMachine.entryTransitions;
                    animatorTransitions.AddRange(transitions);
                }
            }

            return animatorTransitions;
        }

        public static List<AnimatorStateTransition> GetStateTransitions(this AnimatorController controller)
        {
            var states = controller.GetStates();
            var transitions = states.SelectMany(state => state.transitions).ToList();
            return transitions;
        }
        
        public static List<(AnimatorStateTransition, AnimatorState)> GetStateTransitionsWithSource(this AnimatorController controller)
        {
            var states = controller.GetStates();
            var transitions = states.SelectMany(state =>
            {
                return state.transitions.Select(transition => (transition, state));
            }).ToList();
            return transitions;
        }
    }
}