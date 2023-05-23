using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Editor;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

namespace Assets.Scripts.Common
{
	public class ValidateObjectElement: VisualElement
	{
		private readonly List<ValidatePropertyField> _propertyFields = new List<ValidatePropertyField>();

		private readonly SerializedProperty _ownerProperty;

		private readonly Foldout _foldout;

		private StyleSheet _styleSheet;
		private StyleSheet _styleSheetInvalid;

		public ValidateObjectElement(SerializedProperty property)
		{
			_ownerProperty = property;

			_propertyFields.Clear();

			_foldout = new Foldout();

			_styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Common/Editor/ValidateObjectElement.uss");
			_styleSheetInvalid = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Common/Editor/ValidateObjectElementInvalid.uss");

			if (_styleSheet != null)
			{
				_foldout.styleSheets.Add(_styleSheet);
			}

			UpdateDisplayName(property);

			foreach (var childProperty in property.GetVisibleChildren())
			{
				var childField = new ValidatePropertyField();
				childField.Initialize(childProperty, property, PropertyChangeCallback);

				_foldout.Add(childField);

				_propertyFields.Add(childField);
			}

			Add(_foldout);

			CheckPropertiesVisibility();
			CheckValidateProperties();
		}

		protected virtual string GetDisplayName(SerializedProperty property)
		{
			return string.Empty;
		}

		private void UpdateDisplayName(SerializedProperty property)
		{
			var displayName = GetDisplayName(property);
			_foldout.text = !string.IsNullOrWhiteSpace(displayName) ? displayName: property.displayName;
		}

		private void PropertyChangeCallback(SerializedPropertyChangeEvent evt)
		{
			CheckPropertiesVisibility();
			CheckValidateProperties();
			UpdateDisplayName(_ownerProperty);
		}

		private void CheckPropertiesVisibility()
		{
			foreach (var propertyField in _propertyFields)
			{
				bool isPropertyVisible = propertyField.IsVisible();
				propertyField.style.display = new StyleEnum<DisplayStyle>(isPropertyVisible ? DisplayStyle.Flex : DisplayStyle.None);
			}
		}

		private void CheckValidateProperties()
		{
			bool isValid = _propertyFields.Where(p => p.IsVisible()).All(p => p.IsValid());

			if (!isValid)
			{
				if (_styleSheetInvalid != null)
				{
					_foldout.styleSheets.Clear();
					_foldout.styleSheets.Add(_styleSheetInvalid);
				}
			}
			else
			{
				if (_styleSheet != null)
				{
					_foldout.styleSheets.Clear();
					_foldout.styleSheets.Add(_styleSheet);
				}
			}
		}
	}
}
