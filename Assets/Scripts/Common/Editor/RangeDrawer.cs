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
			base.OnGUI(position, property, label);
			return;
			
			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			if (position.height > 16f)
			{
				position.height = 16f;
				EditorGUI.indentLevel += 1;
				contentPosition = EditorGUI.IndentedRect(position);
				contentPosition.y += 18f;
			}

			float half = contentPosition.width / 2;
			GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

			EditorGUIUtility.labelWidth = 28f;

			contentPosition.width *= 0.5f;
			EditorGUI.indentLevel = 0;

			var minProperty = property.FindPropertyRelative("min");
			var maxProperty = property.FindPropertyRelative("max");
			
			EditorGUI.BeginProperty(contentPosition, label, minProperty);
			{
				EditorGUI.BeginChangeCheck();
				EditorGUI.PropertyField(contentPosition, minProperty);
				if (EditorGUI.EndChangeCheck())
				{
					if (minProperty.propertyType == SerializedPropertyType.Integer)
					{
						minProperty.intValue = Math.Min(minProperty.intValue, maxProperty.intValue);
					}
					else if (minProperty.propertyType == SerializedPropertyType.Float)
					{
						minProperty.floatValue = Math.Min(minProperty.floatValue, maxProperty.floatValue);
					}
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
					if (maxProperty.propertyType == SerializedPropertyType.Integer)
					{
						maxProperty.intValue = Math.Max(minProperty.intValue, maxProperty.intValue);
					}
					else if (maxProperty.propertyType == SerializedPropertyType.Float)
					{
						maxProperty.floatValue = Math.Max(minProperty.floatValue, maxProperty.floatValue);
					}
				}
			}
			EditorGUI.EndProperty();
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			//return base.CreatePropertyGUI(property);

			var container = new VisualElement();

			VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Common/Editor/RangeDrawer.uxml");
			visualTree.CloneTree(container);

			//var labelField = container.Q<Label>("label");
			//labelField.text = property.displayName;

			var foldout = container.Q<Foldout>();
			foldout.text = property.displayName;

			var tooltipAttr = property.GetAttribute<TooltipAttribute>();
			if (tooltipAttr != null)
			{
				foldout.tooltip = tooltipAttr.tooltip;
			}

			/*
			var minFieldContainer = container.Q<VisualElement>("minContainer");
			if (minFieldContainer != null)
			{
				var minProperty = property.FindPropertyRelative("min");
				var minField = new FloatField();
				minField.BindProperty(minProperty);
				minField.label = string.Empty;
				minFieldContainer.Add(minField);
			}

			var maxFieldContainer = container.Q<VisualElement>("maxContainer");
			if (maxFieldContainer != null)
			{
				var maxProperty = property.FindPropertyRelative("max");
				var maxField = new FloatField();
				maxField.BindProperty(maxProperty);
				maxField.label = string.Empty;
				maxFieldContainer.Add(maxField);
			}
			*/

			var minField = container.Q<PropertyField>("min");
			minField.BindProperty(property.FindPropertyRelative("min"));
			//minField.label = string.Empty;

			var maxField = container.Q<PropertyField>("max");
			maxField.BindProperty(property.FindPropertyRelative("max"));
			//maxField.label = string.Empty;

			//var minField = container.Q<TextField>("min");
			//minField.BindProperty(property.FindPropertyRelative("min"));

			//var floatField = container.Q<FloatField>("max");

			//floatField.

			/*
			//container.style.backgroundColor = Color.red;

			container.style.flexDirection = FlexDirection.Row;

			var labelField = new TextField(property.displayName);
			container.Add(labelField);

			//property.propertyType == SerializedPropertyType.Float

			var minField = new PropertyField(property.FindPropertyRelative("min"));
			var maxField = new PropertyField(property.FindPropertyRelative("max"));

			//labelField.style.flexGrow = 1.0f;
			//minField.style.flexGrow = 1.0f;
			//maxField.style.flexGrow = 1.0f;

			container.Add(minField);
			container.Add(maxField);

			//container.Add(new HelpBox("FFFFF", HelpBoxMessageType.Error));
			*/

			return container;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return Screen.width < 333 ? (16f + 18f) : 16f;
		}

		/*
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float totalHeight = EditorGUIUtility.singleLineHeight;
 
			var expandable = (property.isExpanded || property.isArray);
			if (expandable)
			{
				while (property.NextVisible(true))
				{
					totalHeight += EditorGUI.GetPropertyHeight(property, label, true);
				}
			}
 
			return expandable ? totalHeight : EditorGUIUtility.singleLineHeight;
 		}
		*/
	}
}
