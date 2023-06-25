public class CommandBase : Command {

    public override string? Input { get; }

    public virtual Action InputAction { get; }
    public override string Description { get; }

    public override void Recognize(string input, Command command) {

        FormatOutput(command);
        InputAction?.Invoke();
    }

    public override bool FormatOutput(ICommandProcessor command) {

        command = this;

        return base.FormatOutput(command);
    }

    public override bool FormatWarning(ICommandProcessor command, DiagnosticCases diagnosticCase) {

        command = this;

        return base.FormatWarning(command, diagnosticCase);
    }

    public override string ToString() {
        return Input;
    }

    public CommandBase(string input, Action action, string description) : base() {
        Input = input;
        InputAction = action;
        Description = description;
    }
}