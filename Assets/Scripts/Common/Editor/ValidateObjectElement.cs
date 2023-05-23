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

		private Color BackColor = new Color(0.3f, 0.3f, 0.35f, 1.0f);

		private static StyleSheet _styleSheet;

		public ValidateObjectElement(SerializedProperty property)
		{
			_ownerProperty = property;

			_propertyFields.Clear();

			_foldout = new Foldout();

			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Common/Editor/ValidateObjectElement.uss");
			if (styleSheet != null)
			{
				_foldout.styleSheets.Add(styleSheet);
			}

			UpdateDisplayName(property);

			foreach (var childProperty in property.GetVisibleChildren())
			{
				var childField = new ValidatePropertyField();
				childField.Initialize(childProperty, property, PropertyChangeCallback);

				childField.style.backgroundColor = new StyleColor(BackColor);

				_foldout.Add(childField);

				_propertyFields.Add(childField);
			}

			Add(_foldout);

			CheckPropertiesVisibility();
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
	}
}
