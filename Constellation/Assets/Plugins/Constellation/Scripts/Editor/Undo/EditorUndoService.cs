using System.Collections.Generic;
using Constellation;
namespace ConstellationEditor {
    [System.Serializable]
    public class EditorUndoService {
        public List<ConstellationScriptData> Actions;
        public int SelectedAction = -1;
        public EditorUndoService () {
            Actions = new List<ConstellationScriptData> ();
        }

        public void AddAction (ConstellationScriptData script) {
            if(SelectedAction != Actions.Count -1)
                ClearActions();

            Actions.Add (new ConstellationScriptData ().Set(script));
            SelectedAction = Actions.Count - 1;
        }

        public ConstellationScriptData Undo () {
            if(Actions.Count == 0 || SelectedAction <= 0)
                return null;

            SelectedAction = SelectedAction -1;
            return Actions[SelectedAction];
        }

        public ConstellationScriptData Redo () {
            if(Actions.Count == 0 || SelectedAction >= Actions.Count -2)
                return null;

            SelectedAction = SelectedAction + 1;
            return Actions[SelectedAction];
        } 

        public void ClearActions() {
            Actions = new List<ConstellationScriptData>();
            SelectedAction = -1;
        }
    }
}