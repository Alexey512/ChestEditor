using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Common
{
	[CustomPropertyDrawer(typeof(RangeInt))]
	[CustomPropertyDrawer(typeof(RangeFloat))]
	public class RangeIntPropertyDrawer: PropertyDrawer
	{
		private const int MaxWidth = 333;
		private const float MaxHeight = 16f;
		private const int Padding = 3;
		private const float LabelWidth = 24f;
		private const int IndentOffset = 2;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			if (position.height > MaxHeight)
			{
				position.height = MaxHeight;
				contentPosition = EditorGUI.IndentedRect(position);
				contentPosition.y += EditorGUIUtility.singleLineHeight;
			}

			EditorGUI.indentLevel -= IndentOffset;

			float half = contentPosition.width * 0.5f;

			float originalLabelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = LabelWidth;

			contentPosition.width *= 0.5f;

			var minProperty = property.FindPropertyRelative("min");
			var maxProperty = property.FindPropertyRelative("max");

			EditorGUI.BeginProperty(contentPosition, label, minProperty);
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(contentPosition, minProperty);
				if (EditorGUI.EndChangeCheck())
				{
					ValidateMinProperty(minProperty, maxProperty);
				}
			}
			EditorGUI.EndProperty();

			contentPosition.x += half;

			EditorGUI.BeginProperty(contentPosition, label, maxProperty);
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(contentPosition, maxProperty);
				if (EditorGUI.EndChangeCheck())
				{
					ValidateMaxProperty(minProperty, maxProperty);
				}
			}

			EditorGUI.EndProperty();

			EditorGUIUtility.labelWidth = originalLabelWidth;

			EditorGUI.indentLevel += IndentOffset;
		}

		protected virtual void ValidateMinProperty(SerializedProperty minProperty, SerializedProperty maxProperty)
		{
			if (minProperty.propertyType == SerializedPropertyType.Integer)
			{
				maxProperty.intValue = Math.Max(minProperty.intValue, maxProperty.intValue);
			}
			else if (minProperty.propertyType == SerializedPropertyType.Float)
			{
				maxProperty.floatValue = Math.Max(minProperty.floatValue, maxProperty.floatValue);
			}
		}

		protected virtual void ValidateMaxProperty(SerializedProperty minProperty, SerializedProperty maxProperty)
		{
			if (maxProperty.propertyType == SerializedPropertyType.Integer)
			{
				minProperty.intValue = Math.Min(minProperty.intValue, maxProperty.intValue);
			}
			else if (maxProperty.propertyType == SerializedPropertyType.Float)
			{
				minProperty.floatValue = Math.Min(minProperty.floatValue, maxProperty.floatValue);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return Screen.width < MaxWidth ? (MaxHeight + EditorGUIUtility.singleLineHeight) : MaxHeight;
		}
	}
}
