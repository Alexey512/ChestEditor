﻿using System;
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

		private readonly PropertyField _propertyField;

		private SerializedProperty _property;

		private SerializedProperty _ownerProperty;

		private readonly HelpBox _helpBox;

		private Action<SerializedPropertyChangeEvent> _valueChangeCallback;

		private readonly List<VisibleIfAttribute> _visibilityAttributes = new List<VisibleIfAttribute>();

		private bool _isRequired = false;

		public ValidatePropertyField()
		{
			_propertyField = new PropertyField();
			Add(_propertyField);
			_propertyField.RegisterValueChangeCallback(OnValueChange);

			_helpBox = new HelpBox();
			Add(_helpBox);
			_helpBox.messageType = HelpBoxMessageType.Error;
			_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
		}

		public void Initialize(SerializedProperty property, SerializedProperty ownerProperty, Action<SerializedPropertyChangeEvent> valueChangeCallback)
		{
			_property = property;
			_ownerProperty = ownerProperty;
			_propertyField.BindProperty(property);
			_valueChangeCallback = valueChangeCallback;

			_isRequired = property.GetAttribute<RequiredPropertyAttribute>() != null;

			_visibilityAttributes.Clear();
			_visibilityAttributes.AddRange(property.GetAttributes<VisibleIfAttribute>());

			ValidateProperty(property);
		}

		public bool IsVisible()
		{
			if (_property == null || _ownerProperty == null)
				return false;

			return _visibilityAttributes.All(attr => EditorReflectionHelper.CheckCondition(_property, attr, _ownerProperty));
		}

		private void ValidateProperty(SerializedProperty property)
		{
			if (_ownerProperty != null)
			{
				_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

				StringBuilder messageText = new StringBuilder();

				var validateAttributes = property.GetAttributes<ValidatePropertyAttribute>();
				foreach (var validateAttribute in validateAttributes)
				{
					if (!EditorReflectionHelper.CheckCondition(property, validateAttribute, _ownerProperty))
					{
						messageText.AppendLine(validateAttribute.message);
					}
				}

				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					if (property.objectReferenceValue == null)
					{
						messageText.AppendLine(property.displayName + " is required");
					}
				}

				if (messageText.Length > 0)
				{
					_helpBox.text = messageText.ToString();
					_helpBox.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
				}
			}
		}

		private void OnValueChange(SerializedPropertyChangeEvent evt)
		{
			ValidateProperty(evt.changedProperty);

			_valueChangeCallback?.Invoke(evt);
		}
	}
}
