namespace Constellation {
		public class EqualCondition : ICondition {
		Ray var1;
		Ray var2;

		public EqualCondition(Ray _var1, Ray _var2)
		{
			var1 = _var1;
			var2 = _var2;
		}
		
		public bool isConditionMet () {
			if(var1.GetString() == var2.GetString())
				return true;
			else 
				return false;
		}
	}
}