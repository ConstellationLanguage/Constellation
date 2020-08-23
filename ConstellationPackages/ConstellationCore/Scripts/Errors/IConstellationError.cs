namespace Constellation {
    public interface IConstellationError {
        string GetErrorMessage ();
        string GetErrorTitle ();
        bool IsIgnorable ();
        string GetFormatedError();
        int GetID ();
        bool IsReportable();
        bool IsCloseEditor();
    }
}