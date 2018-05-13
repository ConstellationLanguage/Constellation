namespace Constellation.CoreNodes {
    public class Condition : INode, IReceiver, IAttributeUpdate {
        private ISender sender;
        
        private Variable var1;
        private Variable var2;
        private Variable var3;

        private Attribute conditionAttribute;
        private Attribute thenAttribute;
        private Attribute elseAttribute;
        private ConditionParser conditon;
        public const string NAME = "Condition";

        public void Setup (INodeParameters _node) {
            var ifValue = new Variable ();
            ifValue.Set("$1==$2");
           
            var thenValue = new Variable ();
            thenValue.Set("$1");

            var elseValue = new Variable();
            elseValue.Set("$2");

            _node.AddInput (this, false, "$1");
            _node.AddInput (this, true, "$2");
            _node.AddInput (this, true, "$3");
            sender = _node.GetSender();
            _node.AddOutput (false, "then");
            _node.AddOutput(false, "else");
            _node.AddOutput(false, "any");

            conditionAttribute = _node.AddAttribute (ifValue, Attribute.AttributeType.Conditionals, "ex: $1>$2");
            thenAttribute = _node.AddAttribute (thenValue, Attribute.AttributeType.Then, "ex: $2");
            elseAttribute = _node.AddAttribute (elseValue, Attribute.AttributeType.Else, "ex: $3");
            var1 = new Variable();
            var2 = new Variable();
            var3 = new Variable();
        } 

        public void OnAttributesUpdate() {
            conditon = null;
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        private void Set () {
            conditon = new ConditionParser(conditionAttribute.Value.GetString (), thenAttribute.Value.GetString(), elseAttribute.Value.GetString(), var1, var2, var3);
        }

        public void Receive (Variable _value, Input _input) {
            if(conditon == null)
                Set();

            if(_input.InputId == 0)
                var1.Set(_value);

            if(_input.InputId == 1)
                var2.Set(_value);

            if(_input.InputId == 2){
                var3.Set(_value);
            }

            if(_input.isWarm) {
                if(conditon.isConditionMet())
                    sender.Send(conditon.ConditionResult(), 0);
                else
                    sender.Send(conditon.ConditionResult(), 1);

                sender.Send(conditon.ConditionResult(), 2);
            }
        }
    }
}