using System;
using Constellation.Variables;
using UnityEngine;

namespace Constellation.Services {
    public class GameObjectService : IGameObject {
        GameObject gameObject;
        Vec3 position;
        Vec3 rotation;
        Vec3 scale;

        public GameObjectService (GameObject _gameObject) {
            gameObject = _gameObject;
            var gameObjectPosition = gameObject.transform.position;
            var gameObjectRotation = gameObject.transform.rotation.eulerAngles;
            var gameObjectScale = gameObject.transform.localScale;
            position = new Vec3 (gameObjectPosition.x, gameObjectPosition.y, gameObjectPosition.z);
            rotation = new Vec3 (gameObjectRotation.x, gameObjectRotation.y, gameObjectRotation.z);
            scale = new Vec3(gameObjectScale.x, gameObjectScale.y, gameObjectScale.z);
        }

        public Vec3 GetPosition () {
            var gameObjectPosition = gameObject.transform.position;
            position.Set(gameObjectPosition.x, gameObjectPosition.y, gameObjectPosition.z);
            return position;
        }

        public Vec3 GetRotation () {
            var gameObjectRotation = gameObject.transform.rotation.eulerAngles;
            rotation.Set(gameObjectRotation.x, gameObjectRotation.y, gameObjectRotation.z);
            return rotation;
        }

        public Vec3 GetScale () {
            var gameObjectScale = gameObject.transform.localScale;
            scale.Set(gameObjectScale.x, gameObjectScale.y, gameObjectScale.z);
            return scale;
        }

        public void SetPosition (float x, float y, float z) {
            throw new NotImplementedException ();
        }

        public void SetRotation (float x, float y, float z) {
            throw new NotImplementedException ();
        }

        public void SetScale (float x, float y, float z) {
            throw new NotImplementedException ();
        }
    }
}