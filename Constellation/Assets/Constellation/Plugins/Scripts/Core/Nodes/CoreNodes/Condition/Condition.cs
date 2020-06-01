using UnityEngine;

namespace Constellation.CoreNodes {
    public class Condition : INode, IReceiver, IParameterUpdate {
        private ISender sender;
        
        private Ray var1;
        private Ray var2;
        private Ray var3;

        private Parameter conditionAttribute;
        private Parameter thenAttribute;
        private Parameter elseAttribute;
        private ConditionParser conditon;
        public const string NAME = "Condition";

        public void Setup (INodeParameters _node) {
            var ifValue = new Ray ();
            ifValue.Set("$1==$2");
           
            var thenValue = new Ray ();
            thenValue.Set("$1");

            var elseValue = new Ray();
            elseValue.Set("$2");

            _node.AddInput (this, false, "$1");
            _node.AddInput (this, true, "$2");
            _node.AddInput (this, true, "$3");
            sender = _node.GetSender();
            _node.AddOutput (false, "then");
            _node.AddOutput(false, "else");
            _node.AddOutput(false, "any");

            conditionAttribute = _node.AddParameter (ifValue, Parameter.ParameterType.Conditionals, "ex: $1>$2");
            thenAttribute = _node.AddParameter (thenValue, Parameter.ParameterType.Then, "ex: $2");
            elseAttribute = _node.AddParameter (elseValue, Parameter.ParameterType.Else, "ex: $3");
            var1 = new Ray();
            var2 = new Ray();
            var3 = new Ray();
        } 

        public void OnParametersUpdate() {
            Set();
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

        public void Receive (Ray _value, Input _input) {
            if(conditon == null)
                Set();

            if(_input.InputId == 0)
                var1.Set(_value);

            if(_input.InputId == 1)
                var2.Set(_value);

            if(_input.InputId == 2){
                var3.Set(_value);
            }

            if(_input.isBright) {
                if(conditon.isConditionMet())
                    sender.Send(conditon.ConditionResult(), 0);
                else
                    sender.Send(conditon.ConditionResult(), 1);

                sender.Send(conditon.ConditionResult(), 2);
            }
        }
    }
}