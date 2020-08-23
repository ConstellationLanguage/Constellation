//  900 FRAMEWORK ISSUE
//900: ConstellationScriptDataDoesNotExist
//901: ScriptNotFoundAtPath
//902: ConstellationNotAddedToFactory

//  000 UNHANDLED ERROR
//000: Not handled error.


//  100 USER MISTAKE
//100: TryingToAccessANullCosntellation
//101: NoConstellationAttached 

namespace Constellation {
    abstract public class ConstellationError : System.Exception {
        abstract public IConstellationError GetError ();
    }
}