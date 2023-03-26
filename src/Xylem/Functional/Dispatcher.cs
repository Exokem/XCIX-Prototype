
using System.Collections.Generic;

namespace Xylem.Functional
{
    public static class UpdateDispatcher
    {
        private static HashSet<Update> _updateReceivers = new HashSet<Update>();
        private static Queue<Update> _queuedUpdates = new Queue<Update>();
        private static Queue<Update> _queuedRenders = new Queue<Update>();

        private static LinkedList<DelayedUpdate> _delayedUpdates = new LinkedList<DelayedUpdate>();

        public static void Register(Update receiver) => _updateReceivers.Add(receiver);
        public static void EnqueueUpdate(Update update) => _queuedUpdates.Enqueue(update);
        public static void EnqueueRenderAction(Update renderAction) => _queuedRenders.Enqueue(renderAction);

        public static void EnqueueDelayedUpdate(Update update, int passes) => _delayedUpdates.AddLast(new DelayedUpdate(update, passes));

        internal static void DispatchUpdates()
        {
            foreach (Update receiver in _updateReceivers)
                receiver();
        }

        internal static void ProcessUpdateQueue()
        {
            while (_queuedUpdates.Count != 0)
                _queuedUpdates.Dequeue()();
        }

        internal static void ProcessRenderQueue()
        {
            while (_queuedRenders.Count != 0)
                _queuedRenders.Dequeue()();
        }

        internal static void ProcessDelayedUpdateQueue()
        {
            LinkedListNode<DelayedUpdate> currentNode = _delayedUpdates.First;
            
            while (currentNode != null)
            {
                LinkedListNode<DelayedUpdate> nextNode = currentNode.Next;

                DelayedUpdate update = currentNode.Value;
                if (update.TryInvoke())
                    _delayedUpdates.Remove(currentNode);

                currentNode = nextNode;
            }
        }
    }

    public class DelayedUpdate
    {
        private readonly Update _update;
        private int _passes;

        public DelayedUpdate(Update update, int passes)
        {
            _update = update;
            _passes = passes;
        }

        public bool TryInvoke()
        {
            if (--_passes == 0)
            {
                _update();
                return true;
            }

            return false;
        }
    }
}