namespace Constellation {
    abstract public class ConstellationError : System.Exception {
        abstract public IConstellationError GetError ();
    }
}