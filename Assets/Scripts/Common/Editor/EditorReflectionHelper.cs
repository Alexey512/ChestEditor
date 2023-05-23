using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using UnityEditor;

namespace Assets.Scripts.Common.Extensions
{
	public static class EditorReflectionHelper
	{
		public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
		{
			SerializedProperty currentProperty = serializedProperty.Copy();
			SerializedProperty nextSiblingProperty = serializedProperty.Copy();
			{
				nextSiblingProperty.NextVisible(false);
			}
 
			if (currentProperty.NextVisible(true))
			{
				do
				{
					if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
						break;

					yield return currentProperty;
				}
				while (currentProperty.NextVisible(false));
			}
		}

		public static bool IsVisible(this SerializedProperty property, SerializedProperty ownerProperty)
		{
			var attribute = property.GetAttribute<VisibleIfAttribute>();
			if (attribute == null)
				return true;

			return CheckCondition(property, attribute, ownerProperty);
		}

		public static bool CheckCondition(this SerializedProperty property, ConditionAttribute attribute, SerializedProperty ownerProperty)
		{
			SerializedProperty conditionField = property.serializedObject.FindProperty(attribute.conditionField);
			if (conditionField == null)
			{
				conditionField = ownerProperty.FindPropertyRelative(attribute.conditionField);
			}
			
			if (conditionField == null)
				return false;

			switch (conditionField.propertyType)
			{
				case SerializedPropertyType.Boolean:
					try
					{
						bool compareValue = attribute.compareValue == null || (bool)attribute.compareValue;
						return conditionField.boolValue == compareValue;
					}
					catch
					{
						return false;
					}
				case SerializedPropertyType.Enum:
					object paramEnum = attribute.compareValue;

					if (paramEnum.GetType().IsEnum)
					{
						string enumValue = Enum.GetValues(paramEnum.GetType()).GetValue(conditionField.enumValueIndex).ToString();
						return paramEnum.ToString() == enumValue;
					}
					else
					{
						return false;
					}
			case SerializedPropertyType.Integer: 
            case SerializedPropertyType.Float:
                string strValue;

                float conditionValue = 0;
                if (conditionField.propertyType == SerializedPropertyType.Integer)
                {
	                conditionValue = conditionField.intValue;
                }
                else if (conditionField.propertyType == SerializedPropertyType.Float)
                {
	                conditionValue = conditionField.floatValue;
                }
                
                try
                {
                    strValue = (string)attribute.compareValue;
                }
                catch
                {
                    return false;
                }

                if (strValue.StartsWith("=="))
                {
                    float? value = GetConditionValue(strValue, "==");
                    return value != null && conditionValue == value;
                }
                else if (strValue.StartsWith("!="))
                {
                    float? value = GetConditionValue(strValue, "!=");
                    return value != null && conditionValue != value;
                }
                else if (strValue.StartsWith("<="))
                {
                    float? value = GetConditionValue(strValue, "<=");
                    return value != null && conditionValue <= value;
                }
                else if (strValue.StartsWith(">="))
                {
                    float? value = GetConditionValue(strValue, ">=");
                    return value != null && conditionValue >= value;
                }
                else if (strValue.StartsWith("<"))
                {
                    float? value = GetConditionValue(strValue, "<");
                    return value != null && conditionValue < value;
                }
                else if (strValue.StartsWith(">"))
                {
                    float? value = GetConditionValue(strValue, ">");
                    return value != null && conditionValue > value;
                }

                break;
			}

			return false;
		}

		public static T GetAttribute<T>(this SerializedProperty property) where T : class
		{
			var attributes = property.GetAttributes<T>();
			return attributes.Length > 0 ? attributes[0] : null;
		}

		public static T[] GetAttributes<T>(this SerializedProperty property) where T : class
		{
			var targetObject = GetTargetObjectByProperty(property);
			if (targetObject == null)
			{
				return new T[] { };
			}

			FieldInfo fieldInfo = targetObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
				.FirstOrDefault(field => field.Name.Equals(property.name, StringComparison.Ordinal));
			if (fieldInfo == null)
			{
				return new T[] { };
			}

			return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
		}

		private static float? GetConditionValue(string content, string remove)
		{
			string removed = content.Replace(remove, "");
			return float.TryParse(removed, out float result) ? result : null;
		}

		private static object GetTargetObjectByProperty(SerializedProperty property)
		{
			object targetObj = property.serializedObject.targetObject;
			string[] items = property.propertyPath.Replace(".Array.data[", "[").Split('.');

			for (int i = 0; i < items.Length - 1; i++)
			{
				string element = items[i];
				if (element.Contains("["))
				{
					int index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
					string itemName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
					targetObj = GetPropertyValue(targetObj, itemName, index);
				}
				else
				{
					targetObj = GetPropertyValue(targetObj, element);
				}
			}

			return targetObj;
		}

		private static object GetPropertyValue(object sourceObj, string name)
		{
			if (sourceObj == null)
			{
				return null;
			}

			Type sourceType = sourceObj.GetType();

			while (sourceType != null)
			{
				FieldInfo field = sourceType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (field != null)
				{
					return field.GetValue(sourceObj);
				}

				PropertyInfo property = sourceType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (property != null)
				{
					return property.GetValue(sourceObj, null);
				}

				sourceType = sourceType.BaseType;
			}

			return null;
		}

		private static object GetPropertyValue(object sourceObj, string name, int index)
		{
			IEnumerable enumerable = GetPropertyValue(sourceObj, name) as IEnumerable;
			if (enumerable == null)
			{
				return null;
			}

			IEnumerator enumerator = enumerable.GetEnumerator();
			for (int i = 0; i <= index; i++)
			{
				if (!enumerator.MoveNext())
				{
					return null;
				}
			}

			return enumerator.Current;
		}
	}
}
