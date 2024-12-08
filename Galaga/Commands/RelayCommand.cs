using System;
using System.Windows.Input;

namespace Galaga.Commands;

/// <summary>
///     Represents a command that can be executed.
/// </summary>
public class RelayCommand : ICommand
{
    #region Data members

    private readonly Action<object> execute;
    private readonly Predicate<object> canExecute;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelayCommand" /> class.
    /// </summary>
    /// <param name="execute">
    ///     The execute action.
    /// </param>
    /// <param name="canExecute">
    ///     The can execute predicate.
    /// </param>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <returns></returns>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    ///     Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command.  If the command does not require data to be passed, this object can
    ///     be set to null.
    /// </param>
    /// <returns>
    ///     true if this command can be executed; otherwise, false.
    /// </returns>
    public bool CanExecute(object parameter)
    {
        return this.canExecute == null || this.canExecute(parameter);
    }

    /// <summary>
    ///     Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">
    ///     Data used by the command.  If the command does not require data to be passed, this object can
    ///     be set to null.
    /// </param>
    public void Execute(object parameter)
    {
        this.execute(parameter);
    }

    #endregion
}