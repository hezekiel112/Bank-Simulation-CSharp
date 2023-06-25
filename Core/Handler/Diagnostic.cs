public class Diagnostic {

    public Dictionary<ICommandProcessor, DiagnosticCases> Diagnostics = new Dictionary<ICommandProcessor, DiagnosticCases>();

    public Diagnostic(Dictionary<ICommandProcessor, DiagnosticCases> diagnostics) {
        Diagnostics = diagnostics;
    }

    public void Add(ICommandProcessor processor, DiagnosticCases diagnosticCase) {
        Diagnostics.Add(processor, diagnosticCase);
    }
}