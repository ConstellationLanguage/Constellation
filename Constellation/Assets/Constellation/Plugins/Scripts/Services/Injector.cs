using System.Collections.Generic;

namespace Constellation
{
    public class Injector : IUpdatable, IAwakable, ILateUpdatable, ITeleportIn
    {
        public List<IUpdatable> updatables;
        public List<IAwakable> Awakables;
        public List<ILateUpdatable> lateUpdatables;
        private Constellation Constellation;
        public List<ITeleportIn> teleportsIn;
        public List<ITeleportOut> teleportsOut;

        public Injector(Constellation constellation)
        {
            Constellation = constellation;
        }

        public void Awake()
        {
            foreach (var awakables in Awakables)
            {
                awakables.OnAwake();
            }
        }


        public void RefreshConstellationEvents()
        {
            updatables = null;
            Awakables = null;
            lateUpdatables = null;
            SetConstellationEvents();
        }

        public void SetConstellationEvents()
        {
            SetAwakables();
            SetTeleports();
            SetUpdatables();
            SetLateUpdatables();
        }

        public void SetAwakables()
        {
            if (Awakables == null)
                Awakables = new List<IAwakable>();

            foreach (var node in Constellation.GetNodes())
            {
                if (node.NodeType as IAwakable != null)
                {
                    Awakables.Add(node.NodeType as IAwakable);
                }
            }
        }

        public void SetUpdatables()
        {
            if (updatables == null)
                updatables = new List<IUpdatable>();

            foreach (var node in Constellation.GetNodes())
            {
                if (node.NodeType as IUpdatable != null)
                {
                    updatables.Add(node.NodeType as IUpdatable);
                }
            }
        }

        public void Update()
        {
            if (updatables == null)
                return;

            foreach (var updatable in updatables)
            {
                updatable.OnUpdate();
            }
        }

        public void LateUpdate()
        {
            foreach (var lateUpdatable in lateUpdatables)
            {
                lateUpdatable.OnLateUpdate();
            }
        }

        public void SetLateUpdatables()
        {
            if (lateUpdatables == null)
                lateUpdatables = new List<ILateUpdatable>();

            foreach (var node in Constellation.GetNodes())
            {
                if (node.NodeType as ILateUpdatable != null)
                {
                    lateUpdatables.Add(node.NodeType as ILateUpdatable);
                }
            }
        }

        public void SubscribeUpdate(IUpdatable _updatable)
        {
            updatables.Add(_updatable);
        }

        public void RemoveUpdatable(IUpdatable _updatable)
        {
            updatables.Remove(_updatable);
        }

        public void OnLateUpdate()
        {
            LateUpdate();
        }

        public void OnAwake()
        {
            Awake();
        }

        public void OnUpdate()
        {
            Update();
        }


        public void SetTeleports()
        {
            SetTeleportsIn();
            SetTeleportsOut();
        }

        public void SetTeleportsIn()
        {

            if (teleportsIn == null)
                teleportsIn = new List<ITeleportIn>();

            foreach (var node in Constellation.GetNodes())
            {
                if (node.NodeType as ITeleportIn != null)
                {
                    var newTeleportIn = node.NodeType as ITeleportIn;
                    teleportsIn.Add(newTeleportIn);
                }
            }
        }

        public void SetTeleportsOut()
        {

            if (teleportsOut == null)
                teleportsOut = new List<ITeleportOut>();

            foreach (var node in Constellation.GetNodes())
            {
                if (node.NodeType as ITeleportOut != null)
                {
                    var newTeleportOut = node.NodeType as ITeleportOut;
                    teleportsOut.Add(newTeleportOut);
                    newTeleportOut.Set(this);
                }
            }
        }

        public void OnTeleport(Variable var, string id)
        {
            foreach (var teleport in teleportsIn)
            {
                teleport.OnTeleport(var, id);
            }
        }
    }
}