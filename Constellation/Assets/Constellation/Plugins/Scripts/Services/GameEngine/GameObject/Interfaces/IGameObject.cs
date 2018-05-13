using Constellation.Variables;

namespace Constellation.Services {
    public interface IGameObject {
         void SetPosition(float x, float y, float z);
         void SetRotation(float x, float y, float z);
         void SetScale(float x, float y, float z);
         Vec3 GetPosition();
         Vec3 GetRotation();
         Vec3 GetScale();
    }
}