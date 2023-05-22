using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class VisibleIfAttribute: ConditionAttribute
	{
		public VisibleIfAttribute(string conditionField, object compareValue = null) : base(conditionField, compareValue)
		{
		}
	}
}
