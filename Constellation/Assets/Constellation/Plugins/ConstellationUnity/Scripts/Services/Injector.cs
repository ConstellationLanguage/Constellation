using System.Collections.Generic;
using Constellation.Services;
using UnityEngine;

namespace Constellation
{
    public class Injector : IUpdatable, IAwakable, ILateUpdatable, ITeleportIn, IFixedUpdate, ICollisionEnter, ICollisionExit, ICollisionStay, IDestroy
    {
        public List<IUpdatable> updatables;
        public List<IAwakable> Awakables;
        public List<ILateUpdatable> lateUpdatables;
        private Constellation Constellation;

        public List<ITeleportIn> teleportsIn;
        public List<ITeleportOut> teleportsOut;
        private List<ICollisionEnter> CollisionEnterListeners;
        private List<ICollisionStay> CollisionStayListeners;
        private List<ICollisionExit> CollisionExitListeners;
        private List<IFixedUpdate> FixedUpdatables;
        private List<IInjectLogger> LoggersInjectors;
        private List<IDestroy> Destroyable;
        private Services.UnityLogger logger;

        public Injector(Constellation constellation)
        {
            logger = new Services.UnityLogger();
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
            CollisionEnterListeners = null;
            CollisionStayListeners = null;
            CollisionExitListeners = null;
            FixedUpdatables = null;
            LoggersInjectors = null;
            SetConstellationEvents();
        }

        public void SetConstellationEvents()
        {
            SetCollisionEnter();
            SetCollisionExit();
            SetCollisionStay();
            SetFixedUpdate();
            SetAwakables();
            SetTeleports();
            SetUpdatables();
            SetLateUpdatables();
            SetInjectLoggers();
            SetDestroyables();
            UpdateLoggers();
        }

        public void SetDestroyables()
        {
            if (Destroyable == null)
                Destroyable = new List<IDestroy>();

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as IDestroy != null)
                {
                    Destroyable.Add(node.NodeType as IDestroy);
                }
            }
        }

        public void SetInjectLoggers()
        {
            if (LoggersInjectors == null)
                LoggersInjectors = new List<IInjectLogger>();

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as IInjectLogger != null)
                {
                    LoggersInjectors.Add(node.NodeType as IInjectLogger);
                }
            }
        }

        public void UpdateLoggers()
        {
            logger = new Services.UnityLogger();
            if (LoggersInjectors == null)
                return;

            foreach (var loggerRequester in LoggersInjectors)
            {
                loggerRequester.InjectLogger(logger);
            }
        }

        public void SetAwakables()
        {
            if (Awakables == null)
                Awakables = new List<IAwakable>();

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
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

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
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

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as ILateUpdatable != null)
                {
                    lateUpdatables.Add(node.NodeType as ILateUpdatable);
                }
            }
        }

        public void SetCollisionStay()
        {
            if (CollisionStayListeners == null)
            {
                CollisionStayListeners = new List<ICollisionStay>();
            }

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as ICollisionStay != null)
                {
                    CollisionStayListeners.Add(node.NodeType as ICollisionStay);
                }
            }
        }

        public void SetCollisionExit()
        {
            if (CollisionExitListeners == null)
            {
                CollisionExitListeners = new List<ICollisionExit>();
            }

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as ICollisionExit != null)
                {
                    CollisionExitListeners.Add(node.NodeType as ICollisionExit);
                }
            }
        }

        public void SetCollisionEnter()
        {
            if (CollisionEnterListeners == null)
            {
                CollisionEnterListeners = new List<ICollisionEnter>();
            }

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as ICollisionEnter != null)
                {
                    CollisionEnterListeners.Add(node.NodeType as ICollisionEnter);
                }
            }
        }

        public void SetFixedUpdate()
        {
            if (FixedUpdatables == null)
                FixedUpdatables = new List<IFixedUpdate>();

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as IFixedUpdate != null)
                {
                    FixedUpdatables.Add(node.NodeType as IFixedUpdate);
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
        public void OnDestroy()
        {
            Destroy();
        }

        public void Destroy()
        {
            foreach (var destroyable in Destroyable)
            {
                destroyable.OnDestroy();
            }
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

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
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

            foreach (var node in Constellation.GetAllNodesAndSubNodes())
            {
                if (node.NodeType as ITeleportOut != null)
                {
                    var newTeleportOut = node.NodeType as ITeleportOut;
                    teleportsOut.Add(newTeleportOut);
                    newTeleportOut.Set(this);
                }
            }
        }

        public void OnTeleport(Ray
            var, string id)
        {
            foreach (var teleport in teleportsIn)
            {
                teleport.OnTeleport(var, id);
            }
        }

        public void OnFixedUpdate()
        {
            foreach (var fixedUpdate in FixedUpdatables)
            {
                fixedUpdate.OnFixedUpdate();
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            foreach (var collisions in CollisionEnterListeners)
            {
                collisions.OnCollisionEnter(collision);
            }
        }

        public void OnCollisionStay(Collision collision)
        {
            foreach (var collisions in CollisionStayListeners)
            {
                collisions.OnCollisionStay(collision);
            }
        }
        public void OnCollisionExit(Collision collision)
        {
            foreach (var collisions in CollisionExitListeners)
            {
                collisions.OnCollisionExit(collision);
            }
        }
    }
}