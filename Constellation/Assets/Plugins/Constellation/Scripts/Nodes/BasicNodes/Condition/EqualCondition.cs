namespace Constellation {
		public class EqualCondition : ICondition {
		Variable var1;
		Variable var2;

		public EqualCondition(Variable _var1, Variable _var2)
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