using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Extensions;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Common.Editor
{
	public class ValidatePropertyField: VisualElement
	{
		public new class UxmlFactory : UxmlFactory<ValidatePropertyField> {}

		private PropertyField _propertyField;

		private SerializedProperty _property;

		private SerializedProperty _ownerProperty;

		private HelpBox _helpBox;

		private Action<SerializedPropertyChangeEvent> _valueChangeCallback;

		private string _propertyName;

		private List<VisibleIfAttribute> _visibilityAttributes = new List<VisibleIfAttribute>();

		public ValidatePropertyField()
		{
			_propertyField = new PropertyField();
			Add(_propertyField);
			_propertyField.RegisterValueChangeCallback(OnValueChange);

			_helpBox = new HelpBox();
			Add(_helpBox);
			_helpBox.messageType = HelpBoxMessageType.Error;
			//_helpBox.visible = false;
			_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
		}

		public void Initialize(SerializedProperty property, SerializedProperty ownerProperty, Action<SerializedPropertyChangeEvent> valueChangeCallback)
		{
			_property = property;
			_propertyName = _property.name;
			_ownerProperty = ownerProperty;
			_propertyField.BindProperty(property);
			_valueChangeCallback = valueChangeCallback;

			_visibilityAttributes.Clear();
			_visibilityAttributes.AddRange(property.GetAttributes<VisibleIfAttribute>());
		}

		public bool IsVisible()
		{
			if (_property == null || _ownerProperty == null)
				return false;

			return _visibilityAttributes.All(attr => EditorExtensions.CheckCondition(_property, attr, _ownerProperty));

			//var property = _ownerProperty.FindPropertyRelative(_propertyName);
			//return property != null && _property.IsVisible(_ownerProperty);
		}

		private void OnValueChange(SerializedPropertyChangeEvent evt)
		{
			if (_ownerProperty != null)
			{
				_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

				var validateAttributes = evt.changedProperty.GetAttributes<ValidatePropertyAttribute>();
				foreach (var validateAttribute in validateAttributes)
				{
					if (!EditorExtensions.CheckCondition(evt.changedProperty, validateAttribute, _ownerProperty))
					{
						_helpBox.text = validateAttribute.message;
						_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
					}
				}
			}

			_valueChangeCallback?.Invoke(evt);
		}
	}
}
