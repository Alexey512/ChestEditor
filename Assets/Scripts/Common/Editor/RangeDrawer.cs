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
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			if (position.height > 16f)
			{
				position.height = 16f;
				contentPosition = EditorGUI.IndentedRect(position);
				contentPosition.y += 18f;
			}

			float half = contentPosition.width / 2;
			GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

			EditorGUIUtility.labelWidth = 28f;

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
			return Screen.width < 333 ? (16f + 18f) : 16f;
		}
	}
}
