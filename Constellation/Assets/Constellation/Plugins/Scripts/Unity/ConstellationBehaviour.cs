using System.Collections.Generic;
using UnityEngine;

namespace Constellation {
    public class ConstellationBehaviour : MonoBehaviour {

        public List<ICollisionEnter> CollisionEnterListeners;
        public List<ICollisionStay> CollisionStayListeners;
        public List<ICollisionExit> CollisionExitListeners;
        public List<BehaviourAttribute> Attributes;
        public List<IFixedUpdate> FixedUpdatables;
        public ConstellationScript ConstellationData;
        private Constellation constellation;
        public static ConstellationEventSystem eventSystem;
        public static bool IsGCDone = false;
        private NodesFactory nodeFactory;
        private bool isInitialized = false;
        private ConstellationError lastConstellationError = null;

        public void Awake () {
            try {
                Initialize ();
            }catch (ConstellationError e) {
                Debug.LogError(e.GetError().GetFormatedError());
            }
        }

        public void HasThrownError(ConstellationError error)
        {
            lastConstellationError = error;
        }

        public ConstellationError GetLastError()
        {
            return lastConstellationError;
        }

        public Constellation GetConstellation () {
            if(constellation == null)
                throw new TryingToAccessANullCosntellation (this);

            return constellation;
        }

        public void Initialize () {
            if (ConstellationData == null && Application.isPlaying) {
                this.enabled = false;
                throw new NoConstellationAttached (this);
            }

            if (isInitialized) // do not initialize twice
                return;

            if (ConstellationBehaviour.eventSystem == null)
                eventSystem = new ConstellationEventSystem ();

            if (NodesFactory.Current == null)
                nodeFactory = new NodesFactory ();
            else
                nodeFactory = NodesFactory.Current;

            var nodes = ConstellationData.GetNodes ();
            constellation = new Constellation ();
            SetNodes (nodes);

            var links = ConstellationData.GetLinks ();
            foreach (LinkData link in links) {
                var input = constellation.GetInput (link.Input.Guid);
                var output = constellation.GetOutput (link.Output.Guid);
                if (input != null && output != null)
                    constellation.AddLink (new Link (constellation.GetInput (link.Input.Guid),
                        constellation.GetOutput (link.Output.Guid),
                        constellation.GetOutput (link.Output.Guid).Type), "none");
            }

            SetUnityObject ();
            SetConstellationEvents ();
            constellation.Initialize (System.Guid.NewGuid ().ToString (), ConstellationData.name);
            constellation.Awake ();
            isInitialized = true;
        }

        public void RefreshConstellationEvents () {
            CollisionEnterListeners = null;
            CollisionStayListeners = null;
            CollisionExitListeners = null;
            FixedUpdatables = null;
            constellation.RefreshConstellationEvents ();
            SetConstellationEvents ();
        }

        public void SetConstellationEvents () {
            constellation.SetConstellationEvents ();
            SetCollisionEnter ();
            SetCollisionExit ();
            SetCollisionStay ();
            SetFixedUpdate ();
        }

        public void RemoveLink (LinkData linkData) {
            Link linkToRemove = null;
            foreach (var link in constellation.Links) {
                if (link.Input.Guid == linkData.Input.Guid && link.Output.Guid == linkData.Output.Guid) {
                    linkToRemove = link;
                }
            }
            linkToRemove.OnDestroy ();
            constellation.Links.Remove (linkToRemove);
        }

        public void AddLink (LinkData link) {
            constellation.AddLink (link);
        }

        public void RemoveNode (NodeData node) {
            constellation.RemovedNode (node.Guid);
        }

        void OnDestroy () {
            if(constellation == null)
                return;

            foreach (var node in constellation.GetNodes ()) {
                if (node.NodeType as IDestroy != null) {
                    node.OnDestroy ();
                }
            }
        }

        public void UpdateAttributes (NodeData[] nodes) {
            var previousAttributes = Attributes;
            Attributes = new List<BehaviourAttribute> ();
            foreach (NodeData node in nodes) {
                if (node == null || previousAttributes == null)
                    return;
                if (node.Name == "ValueAttribute") {
                    var previousAttribute = GetAttributeByName (node.AttributesData[0].Value.GetString (), previousAttributes.ToArray ());
                    if (previousAttribute == null)
                        Attributes.Add (new BehaviourAttribute (new Variable ().Set (0),
                            node.AttributesData[0].Value.GetString (),
                            BehaviourAttribute.Type.Value));
                    else
                        Attributes.Add (previousAttribute);
                } else if (node.Name == "WordAttribute") {
                    var previousAttribute = GetAttributeByName (node.AttributesData[0].Value.GetString (), previousAttributes.ToArray ());
                    if (previousAttribute == null)
                        Attributes.Add (new BehaviourAttribute (new Variable ().Set (0),
                            node.AttributesData[0].Value.GetString (),
                            BehaviourAttribute.Type.Word));
                    else
                        Attributes.Add (previousAttribute);
                } else if (node.Name == "ObjectAttribute") {
                    var previousAttribute = GetAttributeByName (node.AttributesData[0].Value.GetString (), previousAttributes.ToArray ());
                    if (previousAttribute == null)
                        Attributes.Add (new BehaviourAttribute (new Variable ().Set (null as object),
                            node.AttributesData[0].Value.GetString (),
                            BehaviourAttribute.Type.UnityObject));
                    else
                        Attributes.Add (previousAttribute);
                }
            }
        }

        BehaviourAttribute GetAttributeByName (string name, BehaviourAttribute[] attributes) {
            foreach (var attribute in attributes) {
                if (attribute.Name == name)
                    return attribute;
            }
            return null;
        }

        void SetNodes (NodeData[] nodes) {
            var attributesCounter = 0;
            foreach (NodeData node in nodes) {
                var newNode = nodeFactory.GetNode (node);
                constellation.AddNode (newNode, node.Guid, node);
                if (IsAttribute (node) && Attributes != null) {
                    IAttribute nodeAttribute = newNode.NodeType as IAttribute;
                    if (node.Name != "ObjectAttribute" && attributesCounter < Attributes.Count)
                        nodeAttribute.SetAttribute (Attributes[attributesCounter].Variable);
                    else if (attributesCounter < Attributes.Count)
                        nodeAttribute.SetAttribute (new Variable ().Set (Attributes[attributesCounter].UnityObject as object));

                    attributesCounter++;
                }
            }
        }

        public void AddNode (NodeData node) {
            var newNode = nodeFactory.GetNode (node);
            constellation.AddNode (newNode, node.Guid);
            AddUnityObject (newNode);
        }

        bool IsAttribute (NodeData node) {
            if (node.Name == "ValueAttribute" || node.Name == "WordAttribute" || node.Name == "ObjectAttribute")
                return true;

            return false;
        }

        public void SetCollisionStay () {
            if (CollisionStayListeners == null) {
                CollisionStayListeners = new List<ICollisionStay> ();
            }

            foreach (var node in constellation.GetNodes ()) {
                if (node.NodeType as ICollisionStay != null) {
                    CollisionStayListeners.Add (node.NodeType as ICollisionStay);
                }
            }
        }

        public void SetCollisionExit () {
            if (CollisionExitListeners == null) {
                CollisionExitListeners = new List<ICollisionExit> ();
            }

            foreach (var node in constellation.GetNodes ()) {
                if (node.NodeType as ICollisionStay != null) {
                    CollisionExitListeners.Add (node.NodeType as ICollisionExit);
                }
            }
        }

        public void SetCollisionEnter () {
            if (CollisionEnterListeners == null) {
                CollisionEnterListeners = new List<ICollisionEnter> ();
            }

            foreach (var node in constellation.GetNodes ()) {
                if (node.NodeType as ICollisionEnter != null) {
                    CollisionEnterListeners.Add (node.NodeType as ICollisionEnter);
                }
            }
        }

        public void SetFixedUpdate () {
            if (FixedUpdatables == null)
                FixedUpdatables = new List<IFixedUpdate> ();

            foreach (var node in constellation.GetNodes ()) {
                if (node.NodeType as IFixedUpdate != null) {
                    FixedUpdatables.Add (node.NodeType as IFixedUpdate);
                }
            }
        }

        public void SetUnityObject () {
            foreach (var node in constellation.GetNodes ()) {
                AddUnityObject (node);
            }
        }

        public void AddUnityObject (Node<INode> node) {
            if (node.NodeType as IGameObject != null) {
                var igameObject = node.NodeType as IGameObject;
                igameObject.Set (gameObject);
            }
        }

        void Update () {
            if (!IsGCDone) {
                System.GC.Collect ();
                IsGCDone = true;
            }

            constellation.Update ();
        }

        void FixedUpdate () {
            if (FixedUpdatables == null)
                return;
            foreach (var updatable in FixedUpdatables) {
                updatable.OnFixedUpdate ();
            }
        }

        void LateUpdate () {
            IsGCDone = false;
            constellation.LateUpdate ();
        }

        public void Log (Variable value) {
            Debug.Log (value.GetString ());
        }

        void OnCollisionEnter (Collision collision) {
            foreach (var collisions in CollisionEnterListeners) {
                collisions.OnCollisionEnter (collision);
            }
        }

        void OnCollisionStay (Collision collision) {
            foreach (var collisions in CollisionStayListeners) {
                collisions.OnCollisionStay (collision);
            }
        }
        void OnCollisionExit (Collision collision) {
            foreach (var collisions in CollisionExitListeners) {
                collisions.OnCollisionExit (collision);
            }
        }
    }
}