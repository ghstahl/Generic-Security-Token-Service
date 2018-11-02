using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using P7.Core.Reflection;
using System.Text;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Validation;
using P7.Core.Utils;

namespace P7.GraphQLCore.Validators
{

    public class EnterLeaveListenerState
    {

        public EnterLeaveListenerState(OperationType operationType, string currentFieldPath)
        {
            OperationType = operationType;
            CurrentFieldPath = currentFieldPath;
        }
        public FragmentSpread FragmentSpread { get; set; }
        public FragmentDefinition FragmentDefinition { get; set; }
        public OperationType OperationType { get; set; }
        public string CurrentFieldPath { get; set; }
        
    }

     

    public interface IEnterLeaveListenerEventSink
    {
        void OnEvent(EnterLeaveListenerState enterLeaveListenerState);
    }
    public class MyEnterLeaveListener : EventSource<IEnterLeaveListenerEventSink>,INodeVisitor
    {
        private OperationType OperationType { get; set; }


        private Stack<string> _runningPath;

        private Stack<string> RunningPath => _runningPath ?? (_runningPath = new Stack<string>());

        public string CurrentFieldPath
        {
            get { return (RunningPath.Any() ? RunningPath.Peek() : ""); }
        }

        private readonly List<INodeVisitor> _listeners =
            new List<INodeVisitor>();

        public MyEnterLeaveListener(Action<MyEnterLeaveListener> configure)
        {
            configure(this);
        }

        public void Enter(INode node)
        {
            var isField = TypeHelper<Field>.IsType(node.GetType());
            var isOperation = TypeHelper<Operation>.IsType(node.GetType());
            var isFragmentSpread = TypeHelper<FragmentSpread>.IsType(node.GetType());
            var isFragmentDefinition = TypeHelper<FragmentDefinition>.IsType(node.GetType());

            if (isOperation)
            {
                var operation = node as Operation;
                OperationType = operation.OperationType;
                RunningPath.Clear();
                FireEnterLeaveListenerState(new EnterLeaveListenerState(OperationType, CurrentFieldPath));
            }
            if (isField)
            {
                var field = node as Field;
                var next = CurrentFieldPath + "/" + field.Name;
                RunningPath.Push(next);
                FireEnterLeaveListenerState(new EnterLeaveListenerState(OperationType, CurrentFieldPath));
            }

            if (isFragmentSpread)
            {
                var fragmentSpread = node as FragmentSpread;
                FireEnterLeaveListenerState(new EnterLeaveListenerState(OperationType, CurrentFieldPath)
                {
                    FragmentSpread = fragmentSpread
                });
            }

            if (isFragmentDefinition)
            {
                var fragmentDefinition = node as FragmentDefinition;
                FireEnterLeaveListenerState(new EnterLeaveListenerState(OperationType, CurrentFieldPath)
                {
                    FragmentDefinition = fragmentDefinition
                });
            }
            foreach (var listener in _listeners)
            {
                listener.Enter(node);
            }
        }

        public void Leave(INode node)
        {
            var isField = TypeHelper<Field>.IsType(node.GetType());
            if (isField)
            {
                var field = node as Field;
                RunningPath.Pop();
                FireEnterLeaveListenerState(new EnterLeaveListenerState(OperationType, CurrentFieldPath));
            }
            foreach (var listener in _listeners)
            {
                listener.Leave(node);
            }
        }
        public void Match<TNode>(
            Action<TNode> enter = null,
            Action<TNode> leave = null)
            where TNode : INode
        {
            var listener = new MatchingNodeVisitor<TNode>(enter, leave);
            _listeners.Add(listener);
        }

        private void FireEnterLeaveListenerState(EnterLeaveListenerState state)
        {
            foreach (var eventSink in EventSinks)
            {
                eventSink.OnEvent(state);
            }
        }
    }
}
