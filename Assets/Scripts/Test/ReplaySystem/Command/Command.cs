namespace Test.ReplaySystem.Command {
    public interface ICommand {
        void Execute();
        void Undo();
    }
}