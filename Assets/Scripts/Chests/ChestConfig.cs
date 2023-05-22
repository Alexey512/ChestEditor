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


			[VisibleIf("type", RewardType.Hard)]
			[Tooltip("Интервал Hard")]
			public RangeFloat hard;

			[VisibleIf("type", RewardType.Soft)]
			[Tooltip("Интервал Soft")]
			public RangeInt soft;
		}

		//[SerializeField] private RewardInfo _singleReward;

		[SerializeField] private RewardInfo[] _rewards;

		public RewardInfo[] Rewards => _rewards;


	}
}
