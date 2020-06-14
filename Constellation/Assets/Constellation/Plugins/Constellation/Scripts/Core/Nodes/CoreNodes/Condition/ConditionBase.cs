public class ComparisonBase {
    public enum VarTypes{Input, Constant};
    public VarTypes Var1Type;
    public VarTypes Var2Type;
    public Ray Var1;
    public Ray Var2;

    public void Set(Ray var1, VarTypes varTpe, Ray var2, VarTypes var2Type) {
        Var1 = var1;
        Var1Type = varTpe;
        Var2 = var2;
        Var2Type = var2Type;
    }
}