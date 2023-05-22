using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common.Attributes;
using NaughtyAttributes.Editor;
using UnityEditor;

namespace Assets.Scripts.Common.Extensions
{
	public static class EditorExtensions
	{
		/// <summary>
		/// Gets visible children of `SerializedProperty` at 1 level depth.
		/// </summary>
		/// <param name="serializedProperty">Parent `SerializedProperty`.</param>
		/// <returns>Collection of `SerializedProperty` children.</returns>
		public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty, bool includeHidden = false)
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

					//if (includeHidden || currentProperty.IsVisible(serializedProperty.Copy()))
					{
						yield return currentProperty;
					}
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

		public static bool CheckCondition(SerializedProperty property, ConditionAttribute attribute, SerializedProperty ownerProperty)
		{
			SerializedProperty conditionField = property.serializedObject.FindProperty(attribute.conditionField);
			if (conditionField == null)
			{
				conditionField = ownerProperty.FindPropertyRelative(attribute.conditionField);
			}
			
			if (conditionField == null)
				return false;

			//TODO: сделать по новому стилю, refactoring
			switch (conditionField.propertyType)
			{
				case SerializedPropertyType.Boolean:
					try
					{
						bool comparationValue = attribute.compareValue == null || (bool)attribute.compareValue;
						return conditionField.boolValue == comparationValue;
					}
					catch
					{
						//"Invalid comparation Value Type"
						return false;
					}

					break;
				case SerializedPropertyType.Enum:
					object paramEnum = attribute.compareValue;

					if (paramEnum.GetType().IsEnum)
					{
						//if (!CheckSameEnumType(new[] { paramEnum.GetType() },
						//	    property.serializedObject.targetObject.GetType(), conditionField.propertyPath))
						//{
							//"Enum Types doesn't match"
							//return false;
						//}
						//else
						//{
							string enumValue = Enum.GetValues(paramEnum.GetType())
								.GetValue(conditionField.enumValueIndex).ToString();

							return paramEnum.ToString() == enumValue;
						//}
					}
					else
					{
						//"The comparation enum value is not an enum"
						return false;
					}
					break;
			case SerializedPropertyType.Integer: 
            case SerializedPropertyType.Float:
                string stringValue;
                bool error = false;

                float conditionValue = 0;
                if (conditionField.propertyType == SerializedPropertyType.Integer)
                    conditionValue = conditionField.intValue;
                else if (conditionField.propertyType == SerializedPropertyType.Float)
                    conditionValue = conditionField.floatValue;
                
                try
                {
                    stringValue = (string)attribute.compareValue;
                }
                catch
                {
	                //"Invalid comparation Value Type"
                    return false;
                }

                if (stringValue.StartsWith("=="))
                {
                    float? value = GetValue(stringValue, "==");
                    if (value == null)
                        error = true;
                    else
	                    return conditionValue == value;
                }
                else if (stringValue.StartsWith("!="))
                {
                    float? value = GetValue(stringValue, "!=");
                    if (value == null)
                        error = true;
                    else
	                    return conditionValue != value;
                }
                else if (stringValue.StartsWith("<="))
                {
                    float? value = GetValue(stringValue, "<=");
                    if (value == null)
                        error = true;
                    else
	                    return conditionValue <= value;
                }
                else if (stringValue.StartsWith(">="))
                {
                    float? value = GetValue(stringValue, ">=");
                    if (value == null)
                        error = true;
                    else
	                    return conditionValue >= value;
                }
                else if (stringValue.StartsWith("<"))
                {
                    float? value = GetValue(stringValue, "<");
                    if (value == null)
                        error = true;
                    else
	                    return conditionValue < value;
                }
                else if (stringValue.StartsWith(">"))
                {
                    float? value = GetValue(stringValue, ">");
                    if (value == null)
                        error = true;
                    else
                        return conditionValue > value;
                }
                
                if (error)
                {
                    //"Invalid comparation instruction for Int or float value"
                    return false;
                }
                break;
			}

			return false;
		}

		private static float? GetValue(string content, string remove)
		{
			string removed = content.Replace(remove, "");
			try
			{
				return float.Parse(removed);
			}
			catch
			{
				return null;
			}
		}

		public static T GetAttribute<T>(this SerializedProperty property) where T : class
		{
			T[] attributes = property.GetAttributes<T>();
			return (attributes.Length > 0) ? attributes[0] : null;
		}

		public static T[] GetAttributes<T>(this SerializedProperty property) where T : class
		{
			FieldInfo fieldInfo = ReflectionUtility.GetField(GetTargetObjectWithProperty(property), property.name);
			if (fieldInfo == null)
			{
				return new T[] { };
			}

			return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
		}

		public static object GetTargetObjectWithProperty(SerializedProperty property)
		{
			string path = property.propertyPath.Replace(".Array.data[", "[");
			object obj = property.serializedObject.targetObject;
			string[] elements = path.Split('.');

			for (int i = 0; i < elements.Length - 1; i++)
			{
				string element = elements[i];
				if (element.Contains("["))
				{
					string elementName = element.Substring(0, element.IndexOf("["));
					int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue_Imp(obj, elementName, index);
				}
				else
				{
					obj = GetValue_Imp(obj, element);
				}
			}

			return obj;
		}

		private static object GetValue_Imp(object source, string name)
		{
			if (source == null)
			{
				return null;
			}

			Type type = source.GetType();

			while (type != null)
			{
				FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (field != null)
				{
					return field.GetValue(source);
				}

				PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (property != null)
				{
					return property.GetValue(source, null);
				}

				type = type.BaseType;
			}

			return null;
		}

		private static object GetValue_Imp(object source, string name, int index)
		{
			IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
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
