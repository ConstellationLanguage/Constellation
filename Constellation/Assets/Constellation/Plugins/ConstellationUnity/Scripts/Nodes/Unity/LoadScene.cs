namespace Constellation.Unity
{
    public class LoadScene : INode, IReceiver
    {
        public const string NAME = "LoadScene";
        private ISender sender;

        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "Scene Name to load");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(_value.GetString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}