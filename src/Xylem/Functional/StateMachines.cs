using System.Collections.Generic;
using System;

using Xylem.Data;

namespace Xylem.Functional
{
    public sealed class IntervalPublisher<STATEVALUE>
    {
        public delegate void Updater();
        public delegate void Receiver(STATEVALUE stateValue);
        public delegate STATEVALUE StateTransition(STATEVALUE previousStateValue);

        /* The global list of updaters for instantiated publishers. */
        private static readonly List<Updater> _registeredUpdaters = new List<Updater>();
        
        /* The list of update receivers attached to this publisher. */
        private readonly List<Receiver> _receivers = new List<Receiver>();
        /* The interval between state changes. */
        private readonly TimeSpan _interval;
        /* Defines any transitions from one state value to another. */
        private readonly StateTransition _transition;
        /* Stores the default state value for resetting. */
        private readonly STATEVALUE _defaultStateValue;
        /* Updates cannot be forced if this publisher updates automatically. */
        private readonly bool _autoUpdate;

        /* The current state value and the time at which it was assigned. */
        private STATEVALUE _stateValue;
        private DateTime _stateTime = DateTime.MinValue;

        public IntervalPublisher(STATEVALUE baseStateValue, TimeSpan interval, StateTransition transition, bool autoUpdate = true)
        {
            _autoUpdate = autoUpdate;
            _stateValue = _defaultStateValue = baseStateValue;
            _interval = interval;
            _transition = transition;

            if (_autoUpdate)
                _registeredUpdaters.Add(IntrinsicUpdater);
        }

        /* Updates all instantiated Interval Publishers. */
        public static void UpdatePublishers() => _registeredUpdaters.ForEach(updater => updater());

        /* Determines whether a state transition should occur and distributes updates. */
        private void IntrinsicUpdater()
        {
            if (_stateTime == DateTime.MinValue)
                _stateTime = DateTime.Now;

            TimeSpan stateDuration = DateTime.Now - _stateTime;

            if (_interval <= stateDuration)
            {
                // Update the state value using the defined transition
                _stateValue = _transition(_stateValue);
                // Notify update receivers
                NotifyReceivers();
                // Reset state time
                _stateTime = DateTime.Now;
            }
        }

        private void NotifyReceivers() => _receivers.ForEach(receiver => receiver(_stateValue));

        /* Updates can only be forced if this publisher is not automated. */
        public void ForceUpdate()
        {
            if (!_autoUpdate)
                IntrinsicUpdater();
            else throw new InvalidOperationException("Cannot force updates on an automatically updated interval publisher");
        }

        public void AttachReceiver(Receiver receiver) => _receivers.Add(receiver);

        public void Reset()
        {
            _stateTime = DateTime.Now;
            _stateValue = _defaultStateValue;
            NotifyReceivers();
        }
    }
}