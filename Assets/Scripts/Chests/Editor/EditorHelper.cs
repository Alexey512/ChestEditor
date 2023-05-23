using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Chests
{
	public static class EditorHelper
	{
		public static string GetRewardInfoDisplayName(SerializedProperty property)
		{
			int arrayIndex = -1;
			var propertyPath = property.propertyPath;
			int indexStart = propertyPath.LastIndexOf(".Array.data[", StringComparison.Ordinal);
			if (indexStart >= 0)
			{
				indexStart += 12;
				int indexEnd = propertyPath.LastIndexOf("]", StringComparison.Ordinal);
				if (indexEnd >= 0 && indexEnd > indexStart)
				{
					int.TryParse(propertyPath.Substring(indexStart, indexEnd - indexStart), out arrayIndex);
				}
			}

			string[] items = property.propertyPath.Replace(".Array.data[", "[").Split('.');
			
			var typeProperty = property.FindPropertyRelative("type");
			ChestConfig.RewardType rewardType = (ChestConfig.RewardType)typeProperty.enumValueIndex;
			
			float weight = property.FindPropertyRelative("randomWeight").floatValue;

			string displayName = string.Empty;
			switch (rewardType)
			{
				case ChestConfig.RewardType.Chest:
					var chestProperty = property.FindPropertyRelative("chest");
					var nestedChest = chestProperty.objectReferenceValue as ChestConfig;
					displayName = nestedChest != null ?
						$"Type={rewardType}; Weight={weight}; Name={nestedChest.name}; Count={property.FindPropertyRelative("chestCount").intValue}" :
						$"Type={rewardType}; Weight={weight}; Count={property.FindPropertyRelative("chestCount").intValue}";
					break;
				case ChestConfig.RewardType.Hard:
					displayName = $"Type={rewardType}; Weight={weight}; Range=[{property.FindPropertyRelative("hardRange.min").floatValue}-{property.FindPropertyRelative("hardRange.max").floatValue}]";
					break;
				case ChestConfig.RewardType.Soft:
					displayName = $"Type={rewardType}; Weight={weight}; Range=[{property.FindPropertyRelative("softRange.min").intValue}-{property.FindPropertyRelative("softRange.max").intValue}]";
					break;
			}

			if (string.IsNullOrWhiteSpace(displayName))
				return string.Empty;

			return arrayIndex >= 0 ? $"{arrayIndex}: {displayName}" : displayName;
		}
	}
}
