
using System;

namespace Xylem.Functional
{
    public struct ReversibleAction
    {
        private readonly Action _action, _inverse;
        private readonly string _description;

        public ReversibleAction(Action action, Action inverse, string description = "")
        {
            _action = action;
            _inverse = inverse;
            _description = description;
        }

        public void Invoke() => _action();
        public void Revert() => _inverse();

        public override string ToString()
        {
            return _description.Length == 0 ? "Unknown action" : _description;
        }
    }
}