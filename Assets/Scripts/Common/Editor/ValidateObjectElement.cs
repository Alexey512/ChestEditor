using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Editor;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Assets.Scripts.Common
{
	public class ValidateObjectElement: VisualElement
	{
		private readonly List<ValidatePropertyField> _propertyFields = new List<ValidatePropertyField>();

		private SerializedProperty _ownerProperty;

		public ValidateObjectElement(SerializedProperty property)
		{
			_ownerProperty = property;

			_propertyFields.Clear();

			//var container = new VisualElement();

			var foldout = new Foldout();
			foldout.text = property.displayName;

			foreach (var childProperty in property.GetVisibleChildren(true))
			{
				var childField = new ValidatePropertyField();
				childField.Initialize(childProperty, property, PropertyChangeCallback);

				foldout.Add(childField);

				_propertyFields.Add(childField);

				//childField.IsVisible();
			}

			//container.Add(foldout);

			Add(foldout);

			CheckPropertiesVisibility();
		}

		private void PropertyChangeCallback(SerializedPropertyChangeEvent evt)
		{
			CheckPropertiesVisibility();
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
