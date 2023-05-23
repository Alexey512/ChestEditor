using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class ValidatePropertyAttribute: ConditionAttribute
	{
		public readonly string message;
		
		public ValidatePropertyAttribute(string message, string conditionField, object compareValue = null) : base(conditionField, compareValue)
		{
			this.message = message;
		}
	}
}
