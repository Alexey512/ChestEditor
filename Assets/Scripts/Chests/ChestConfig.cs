using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Attributes;
using NaughtyAttributes;
using UnityEngine;
using RangeInt = Assets.Scripts.Common.RangeInt;

namespace Assets.Scripts.Chests
{
	[CreateAssetMenu(menuName = "Inventory/Chest")]
	public class ChestConfig: ScriptableObject 
	{

		public enum RewardType 
		{
			Soft,
			Hard,
			Chest,
		}

		[Serializable]
		public class RewardInfoAdd 
		{
			[Range(0, 1)]
			public float randomWeight = 1;
			
			public RewardType type;
			
			public RangeFloat hard;
		}

		[Serializable]
		public class RewardInfo
		{
			[HideInInspector]
			public string name;

			[Range(0, 1)]
			[Tooltip("Шанс!")]
			public float randomWeight = 1;
			
			public RewardType type;
			
			//public int softMin;
			//public int softMax;
			//public float hardMin;
			//public float hardMax;

			[VisibleIf("type", RewardType.Chest)]
			[RequiredProperty]
			public ChestConfig chest;
			[VisibleIf("type", RewardType.Chest)]
			[ValidateProperty("Not set chests count", "chestCount", ">0")]
			public int chestCount;


			//[VisibleIf("type", RewardType.Hard)]
			//public RangeFloat hard;

			//[VisibleIf("type", RewardType.Soft)]
			//public RangeInt soft;

			//public Vector2 vector2;
			//public Point point;

			//[SerializeField]
			//public RewardInfoAdd info;
		}

		//[SerializeField] private RewardInfo _singleReward;

		[SerializeField] private RewardInfo[] _rewards;
		

		//[SerializeField] private RangeFloat[] _ranges;

		//[SerializeField] private Point[] _points;

		//[SerializeField] private Ingredient[] _ingredients;

		public RewardInfo[] Rewards => _rewards;


	}
}
