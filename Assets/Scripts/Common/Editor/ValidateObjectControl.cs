using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Common
{
	public class ValidateObjectControl
	{
		
		public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect foldoutRect = new Rect()
			{
				x = position.x,
				y = position.y,
				width = EditorGUIUtility.labelWidth,
				height = EditorGUIUtility.singleLineHeight
			};

			var displayName = GetDisplayName(property);

			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, !string.IsNullOrWhiteSpace(displayName) ? new GUIContent(displayName) : label);

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
						if (!EditorReflectionHelper.CheckCondition(childProperty, validateAttribute, property))
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
		}

		public float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float totalHeight = EditorGUIUtility.singleLineHeight;

			var expandable = (property.isExpanded || property.isArray);
			if (expandable)
			{
				foreach (var childProperty in property.GetVisibleChildren())
				{
					if (!childProperty.IsVisible(property))
						continue;
					
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
						if (!EditorReflectionHelper.CheckCondition(childProperty, validateAttribute, property))
						{
							totalHeight += EditorGUIUtility.singleLineHeight * 2;
						}
					}
				}
			}

			return expandable ? totalHeight : EditorGUIUtility.singleLineHeight;
		}

		protected virtual string GetDisplayName(SerializedProperty property)
		{
			return string.Empty;
		}
	}
}
