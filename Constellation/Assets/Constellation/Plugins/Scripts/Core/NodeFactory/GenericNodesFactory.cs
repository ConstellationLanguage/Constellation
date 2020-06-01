using System;
using System.Linq;
using System.Reflection;

namespace Constellation
{
    public static class GenericNodeFactory
    {
        public static System.Type[] GetNodes()
        {
            var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.IsSubclassOf(typeof(Node<INode>))
                             select type;

            return subclasses.ToArray();
        }

        public static String[] GetNodesTypeExcludeDiscretes()
        {
            var subclasses = from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.GetInterfaces().Contains(typeof(INode))
                             where !t.GetInterfaces().Contains(typeof(IDiscreteNode))
                                      && t.GetConstructor(Type.EmptyTypes) != null
                             select t.ToString();

            return subclasses.ToArray();
        }

        public static String[] GetNodesType()
        {
            var subclasses = from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.GetInterfaces().Contains(typeof(INode))
                                      && t.GetConstructor(Type.EmptyTypes) != null
                             select t.ToString();

            return subclasses.ToArray();
        }

        public static System.Type[] GetNode()
        {
            var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.IsSubclassOf(typeof(Node<INode>))
                             select type;

            return subclasses.ToArray();
        }
    }
}
