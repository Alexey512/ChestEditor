using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Editor;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Chests
{
	[CustomPropertyDrawer(typeof(ChestConfig.RewardInfo))]
	public class RewardInfoDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
			return;

			//Rect foldOutRect = new Rect(position.xMin, position.yMin, position.size.x,
			//	EditorGUIUtility.singleLineHeight);
			
			Rect foldoutRect = new Rect()
			{
				x = position.x,
				y = position.y,
				width = EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);

			if (property.isExpanded)
			{
				EditorGUI.indentLevel++;

				float yOffset = EditorGUIUtility.singleLineHeight;

				foreach (var childProperty in property.GetVisibleChildren())
				{
					if (!childProperty.IsVisible(property))
						continue;
					
					float childHeight = EditorGUI.GetPropertyHeight(childProperty, label, true);
					Rect childRect = new Rect()
					{
						x = position.x,
						y = position.y + yOffset,
						width = position.width,
						height = childHeight
					};
					
					EditorGUI.BeginProperty(childRect, label, childProperty);
					EditorGUI.PropertyField(childRect, childProperty, true);
					
					yOffset += childHeight;

					var requiredAttribute = childProperty.GetAttribute<RequiredPropertyAttribute>();
					if (requiredAttribute != null)
					{
						if (childProperty.propertyType == SerializedPropertyType.ObjectReference)
						{
							if (childProperty.objectReferenceValue == null)
							{
								string errorMessage = childProperty.displayName + " is required";

								Rect boxRect = new Rect()
								{
									x = position.x,
									y = position.y + yOffset,
									width = position.width,
									height = EditorGUIUtility.singleLineHeight * 2
								};
								EditorGUI.HelpBox(boxRect, errorMessage, MessageType.Error);
								yOffset += EditorGUIUtility.singleLineHeight * 2;
							}
						}
					}

					var validateAttributes = childProperty.GetAttributes<ValidatePropertyAttribute>();
					foreach (var validateAttribute in validateAttributes)
					{
						if (!EditorExtensions.CheckCondition(childProperty, validateAttribute, property))
						{
							Rect boxRect = new Rect()
							{
								x = position.x,
								y = position.y + yOffset,
								width = position.width,
								height = EditorGUIUtility.singleLineHeight * 2
							};
							EditorGUI.HelpBox(boxRect, validateAttribute.message, MessageType.Error);
							yOffset += EditorGUIUtility.singleLineHeight * 2;
						}
					}

					EditorGUI.EndProperty();
				}

				EditorGUI.indentLevel--;
			}

			property.serializedObject.ApplyModifiedProperties();
			//EditorGUI.EndProperty();
		}

		private void DrawHelpBox(string message)
		{
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			//return base.GetPropertyHeight(property, label);
			
			float totalHeight = EditorGUIUtility.singleLineHeight;

			var expandable = (property.isExpanded || property.isArray);
			if (expandable)
			{
				//TODO: оптимизировать
				foreach (var childProperty in property.GetVisibleChildren())
				{
					totalHeight += EditorGUI.GetPropertyHeight(childProperty, label, true);

					var requiredAttribute = childProperty.GetAttribute<RequiredPropertyAttribute>();
					if (requiredAttribute != null)
					{
						if (childProperty.propertyType == SerializedPropertyType.ObjectReference && childProperty.objectReferenceValue == null)
						{
							totalHeight += EditorGUIUtility.singleLineHeight * 2;
						}
					}

					var validateAttributes = childProperty.GetAttributes<ValidatePropertyAttribute>();
					foreach (var validateAttribute in validateAttributes)
					{
						if (!EditorExtensions.CheckCondition(childProperty, validateAttribute, property))
						{
							totalHeight += EditorGUIUtility.singleLineHeight * 2;
						}
					}
				}
			}

			return expandable ? totalHeight : EditorGUIUtility.singleLineHeight;
		}

		//private Dictionary<Seria>

		private readonly List<ValidatePropertyField> _propertyFields = new List<ValidatePropertyField>();

		private SerializedProperty _ownerProperty;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new ValidateObjectElement(property);
			//return base.CreatePropertyGUI(property);

			_ownerProperty = property;

			_propertyFields.Clear();

			var container = new VisualElement();

			var foldout = new Foldout();
			foldout.text = property.displayName;

			foreach (var childProperty in property.GetVisibleChildren(true))
			{
				var childField = new ValidatePropertyField();
				childField.Initialize(childProperty, property, PropertyChangeCallback);

				foldout.Add(childField);

				_propertyFields.Add(childField);

				childField.IsVisible();
			}

			container.Add(foldout);

			//CheckPropertiesVisibility();

			return container;
		}

		private void PropertyChangeCallback(SerializedPropertyChangeEvent evt)
		{
			//CheckPropertiesVisibility();
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
