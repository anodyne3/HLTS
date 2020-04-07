using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Input
{
    public struct PointerInput
    {
        public bool contact;
        public int inputId;
        public Vector2 position;
    }

    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class PointerInputComposite : InputBindingComposite<PointerInput>
    {
        [InputControl(layout = "Button")]
        public int contact;

        [InputControl(layout = "Vector2")]
        public int position;

        [InputControl(layout = "Integer")]
        public int inputId;

        public override PointerInput ReadValue(ref InputBindingCompositeContext context)
        {
            var contact = context.ReadValueAsButton(this.contact);
            var pointerId = context.ReadValue<int>(inputId);
            var position = context.ReadValue<Vector2, Vector2MagnitudeComparer>(this.position);

            return new PointerInput
            {
                contact = contact,
                inputId = pointerId,
                position = position,
            };
        }

        #if UNITY_EDITOR
        static PointerInputComposite()
        {
            Register();
        }

        #endif

        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            InputSystem.RegisterBindingComposite<PointerInputComposite>();
        }
    }
}
