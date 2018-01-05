Any help or idea is welcome. If you don't know where to start go check the issues tab and find one labeled for beginners.

CREATING A NEW NODE (Use the Var.cs node as a reference, it's one of the most basic ones and easy to understand)
- Node should implement INode, IReceiver interfaces
- Node input or output should be set in the setup() function.
- Node should be in inside one of the following namespaces: Constellation.Math, Constellation.UI, Constellation.Unity, Constellation.BasicNodes, Constellation.Physics
- Stock nodes should use the node Factory to be instantiated and should be set inside the one linked to it's namespace. 
    Ex: You create a MousePosition.cs class you should add it to Constellation.Unity namespace and instantiate it inside UnityNodeFactory.cs

CREATING A NODE EXAMPLE.
- Create a new constellation using the editor in .\Constellation\Assets\Plugins\Constellation\Examples\.
- It should have a note node explaining quickly what the node is about.
- A working example that print an output in the console.

CREATING AN EXAMPLE PROJECT.
- The assets should conform to the unity asset store submission requirements.
