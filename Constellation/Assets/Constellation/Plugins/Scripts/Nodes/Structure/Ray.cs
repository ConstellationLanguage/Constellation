[System.Serializable]
public class Ray {
    public string stringValue;
    public const float nullValue = 0.000001f;
    public float floatValue;

    [System.NonSerialized]
    private Ray[] Variables;
    private object Object;

    public Ray () { }

    public Ray (string value) {
        Set (value);
    }

    public Ray (float value) {
        Set (value);
    }

    public Ray (Ray variable) {
        Set (variable);
    }

    public Ray (Ray[] variables) {
        Set (variables);
    }

    public Ray (object value) {
        Set (value);
    }

    public Ray Set (string value) {
        UnsetAll ();
        stringValue = value;
        return this;
    }

    public Ray Set (float value) {
        UnsetAll ();
        floatValue = value;
        return this;
    }

    public Ray Set (Ray variable) {
        UnsetAll ();
        if (variable.IsFloat ())
            Set (variable.GetFloat ());
        else if (variable.IsString ())
            Set (variable.GetString ());
        else if (variable.GetObject () != null)
            Set (variable.GetObject ());
        else if (variable.GetArray () != null)
            Set (variable.GetArray ());

        return this;
    }

    public Ray Set (Ray[] variables) {
        UnsetAll ();
        if (variables != null)
            Variables = new Ray[variables.Length];
        else 
            return this;

        for (var i = 0; i < variables.Length; i++) {
            Variables[i] = new Ray (variables[i]);
        }
        return this;
    }

    public Ray Set (object _object) {
        UnsetAll ();
        Object = _object;
        return this;
    }

    public void SetAtIndex (float value, int index) {
        if (Variables.Length > index && index > -1)
            Variables[index] = Variables[index].Set (value);
    }

    public string GetString (int index = 0) {
        if (stringValue != "UNDEFINED")
            return stringValue;
        else if (floatValue != nullValue)
            return floatValue.ToString ();
        else if (Variables == null) {
            if (Object == null)
                return null;
            else
                return Object.ToString ();
        } else if (Variables.Length > index) {
            return Variables[index].GetString ();
        }

        return null;
    }

    public Ray[] GetArray () {
        if (Variables == null) {
            return null;
        }

        return Variables;
    }

    public Ray GetArrayVariable (int index) {
        if (Variables == null)
            return this;

        if (Variables.Length > index)
            return Variables[index];

        return this;
    }

    public float GetFloat (int index = 0) {
        if (stringValue != "UNDEFINED" && float.TryParse (stringValue, out floatValue)) {
            return floatValue;
        } else if (floatValue != nullValue)
            return floatValue;
        else if (Variables != null) {
            if (Variables.Length > index)
                return Variables[index].GetFloat ();
        }
        return nullValue;
    }

    public object GetObject () {
        if (Object != null)
            return Object;
        else if (stringValue != "UNDEFINED")
            return stringValue;
        else if (floatValue != nullValue)
            return floatValue;
        else
            return null;
    }

    public bool IsFloat () {
        if (stringValue == "UNDEFINED" && stringValue != "Ray" && Variables == null && floatValue != nullValue) {
            return true;
        }

        return false;
    }

    public bool IsString () {
        if (stringValue != "UNDEFINED" && stringValue != "Ray" && Variables == null) {
            return true;
        }

        return false;
    }

    private void UnsetAll () {
        Variables = null;
        stringValue = "UNDEFINED";
        floatValue = nullValue;
        Object = null;
    }

}