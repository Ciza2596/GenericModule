using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CizaInputModule.Editor
{
    [CustomEditor(typeof(RebindActionUI))]
    public class RebindActionUIEditor : UnityEditor.Editor
    {
        private SerializedProperty _inputActionReferenceProperty;
        private SerializedProperty _controlSchemeProperty;
        private SerializedProperty _bindingIdProperty;

        private readonly GUIContent _bindingLabel = new GUIContent("Binding");
        private GUIContent[] _bindingOptionLabels;

        private string[] _bindingIds;
        private int _bindingOptionIndex = -1;

        protected void OnEnable()
        {
            _inputActionReferenceProperty = serializedObject.FindProperty("_inputActionReference");
            _controlSchemeProperty = serializedObject.FindProperty("_controlScheme");
            _bindingIdProperty = serializedObject.FindProperty("_bindingId");

            RefreshBindingOptions();
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_inputActionReferenceProperty);

            var bindingOptionIndex = EditorGUILayout.Popup(_bindingLabel, _bindingOptionIndex, _bindingOptionLabels);
            if (bindingOptionIndex != _bindingOptionIndex)
            {
                var bindingId = _bindingIds[bindingOptionIndex];
                _bindingIdProperty.stringValue = bindingId;
                _bindingOptionIndex = bindingOptionIndex;

                if (TryGetAction(out var action))
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                        _controlSchemeProperty.stringValue = GetControlScheme(action.bindings[bindingOptionIndex], asset);
                }
            }
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_controlSchemeProperty);
            GUI.enabled = true;


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                RefreshBindingOptions();
            }
        }

        private void RefreshBindingOptions()
        {
            if (!TryGetAction(out var action))
            {
                _bindingOptionLabels = Array.Empty<GUIContent>();
                _bindingIds = Array.Empty<string>();
                _bindingOptionIndex = -1;
                return;
            }

            var bindings = action.bindings;
            var bindingCount = bindings.Count;

            _bindingOptionLabels = new GUIContent[bindingCount];
            _bindingIds = new string[bindingCount];
            _bindingOptionIndex = -1;

            var currentBindingId = _bindingIdProperty.stringValue;
            for (var i = 0; i < bindingCount; ++i)
            {
                var binding = bindings[i];
                var bindingId = binding.id.ToString();
                var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                // Create display string.
                var displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        var controlSchemes = GetControlScheme(binding, asset);
                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                _bindingOptionLabels[i] = new GUIContent(displayString);
                _bindingIds[i] = bindingId;

                if (currentBindingId == bindingId)
                    _bindingOptionIndex = i;
            }
        }

        private bool TryGetAction(out InputAction action)
        {
            var actionReference = (InputActionReference)_inputActionReferenceProperty.objectReferenceValue;
            action = actionReference?.action;
            return action != null;
        }

        private string GetControlScheme(InputBinding binding, InputActionAsset asset) =>
            string.Join(", ", binding.groups.Split(InputBinding.Separator).Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));
    }
}