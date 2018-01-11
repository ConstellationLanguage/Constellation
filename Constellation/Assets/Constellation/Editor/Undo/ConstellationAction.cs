namespace ConstellationEditor {
    public class ConstellationAction {
        public enum ActionType {AddNode, RemoveNode, AddLink, RemoveLink};
        public readonly ActionType Type;
        public readonly string ActionData;

        public ConstellationAction(ActionType _actionType, string _actionData) {
            Type = _actionType;
            ActionData = _actionData;
        }
    }
}