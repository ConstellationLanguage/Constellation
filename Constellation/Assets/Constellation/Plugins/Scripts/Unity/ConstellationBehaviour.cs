using System.Collections.Generic;
using UnityEngine;

namespace Constellation {
    public class ConstellationBehaviour : ConstellationEditable {

        private List<ICollisionEnter> CollisionEnterListeners;
        private List<ICollisionStay> CollisionStayListeners;
        private List<ICollisionExit> CollisionExitListeners;
        private List<IFixedUpdate> FixedUpdatables;

        public void Awake () {
            try {
                if (ConstellationData == null && Application.isPlaying) {
                 ConstellationData = ScriptableObject.CreateInstance<ConstellationScript>();
                 ConstellationData.InitializeData();
                 ConstellationData.IsInstance = true;
                }
                Initialize ();
            }catch (ConstellationError e) {
                Debug.LogError(e.GetError().GetFormatedError());
            }
        }

        public override void RefreshConstellationEvents () {
            CollisionEnterListeners = null;
            CollisionStayListeners = null;
            CollisionExitListeners = null;
            FixedUpdatables = null;
            constellation.RefreshConstellationEvents ();
            SetConstellationEvents ();
        }

        protected override void SetConstellationEvents () {
            constellation.SetConstellationEvents ();
            SetCollisionEnter ();
            SetCollisionExit ();
            SetCollisionStay ();
            SetFixedUpdate ();
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