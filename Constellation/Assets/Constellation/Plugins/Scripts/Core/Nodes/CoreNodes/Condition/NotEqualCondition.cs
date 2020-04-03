namespace Constellation {
	public class NotEqualCondition : ICondition {
		Ray var1;
		Ray var2;

		public NotEqualCondition (Ray _var1, Ray _var2) {
			var1 = _var1;
			var2 = _var2;
		}

		public bool isConditionMet () {
			if (var1.GetString () != var2.GetString ())
				return true;
			else
				return false;
		}
	}
}