using UnityEngine;
using System;

namespace Constellation.Unity3D
{
    public class ConstellationBehaviour : ConstellationEditable
    {
        public void Awake()
        {
            /*try
            {*/
                if (ConstellationData == null && Application.isPlaying)
                {
                    this.enabled = false;
                }
                else
                {
                    Initialize();
                }
            /*}
            catch (ConstellationError e)
            {
                Debug.LogError(e.GetError().GetFormatedError());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }*/
        }

        public void SetConstellation(ConstellationScriptData constellationScriptData)
        {
            ConstellationData = ScriptableObject.CreateInstance<ConstellationScript>();
            ConstellationData.script = constellationScriptData;
            ConstellationData.IsInstance = true;
            Initialize();
            this.enabled = true;
        }

        void OnDestroy()
        {
            if (constellation.GetInjector() is IDestroy)
                constellation.GetInjector().OnDestroy();
        }

        void Update()
        {
            if (!IsGCDone && Time.frameCount % 10 == 0)
            {
                //System.GC.Collect();
                IsGCDone = true;
            }
            if (constellation.GetInjector() is IUpdatable)
                constellation.GetInjector().Update();
        }

        void FixedUpdate()
        {
            if (constellation.GetInjector() is IFixedUpdate)
                constellation.GetInjector().OnFixedUpdate();
        }

        void LateUpdate()
        {
            IsGCDone = false;
            if (constellation.GetInjector() is ILateUpdatable)
                constellation.GetInjector().LateUpdate();
        }

        public void Log(Ray value)
        {
            Debug.Log(value.GetString());
        }

        void OnCollisionEnter(Collision collision)
        {
            if (constellation.GetInjector() is ICollisionEnter)
                constellation.GetInjector().OnCollisionEnter(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            if (constellation.GetInjector() is ICollisionStay)
                constellation.GetInjector().OnCollisionStay(collision);
        }
        void OnCollisionExit(Collision collision)
        {
            if (constellation.GetInjector() is ICollisionExit)
                constellation.GetInjector().OnCollisionExit(collision);
        }
    }
}