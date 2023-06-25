public interface ICommandProcessor {

    public abstract bool FormatOutput(ICommandProcessor command);

    public abstract bool FormatWarning(ICommandProcessor command, DiagnosticCases diagnosticCase);
}