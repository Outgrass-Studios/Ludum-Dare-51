using System;
using UnityEngine;

namespace qASIC.InputManagement.Map
{
    [Serializable]
    public class Input1DAxis : InputMapItem<float>
    {
        public Input1DAxis() : base() { }
        public Input1DAxis(string name) : base(name) { }

        public string positiveAction;
        public string negativeAction;

        public override float ReadValue(Func<string, float> func)
        {
            InputBinding positive = Map.GetItem<InputBinding>(positiveAction);
            InputBinding negative = Map.GetItem<InputBinding>(negativeAction);

            float positiveValue = positive == null ? 0f : positive.ReadValue(func);
            float negativeValue = negative == null ? 0f : negative.ReadValue(func);

            return positiveValue - negativeValue;
        }

        public override InputEventType GetInputEvent(Func<string, InputEventType> func)
        {
            InputBinding positive = Map.GetItem<InputBinding>(positiveAction);
            InputBinding negative = Map.GetItem<InputBinding>(negativeAction);

            return positive.GetInputEvent(func) | negative.GetInputEvent(func);
        }

        public override float GetHighestValue(float a, float b) =>
            Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
    }
}