using System;
using System.Windows.Input;

namespace MVVMToolkit.Commands
{
    /// <summary>
    /// Defines a relay command with Execute and Can Execute delegate with no parameters
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        /// <summary>
        /// the method that determines whether the command can execute in its current state.
        /// </summary>
        readonly Action _execute;

        /// <summary>
        /// the method that determines whether the command can execute in its current state.
        /// </summary>
        readonly Func<bool> _canExecute;

        /// <summary>
        /// <para>Represents the method that will handle an event that has no event data.</para>
        /// <para>And subscribe the method to CommandManager.RequerySuggested.</para>
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { if (_canExecute != null) CommandManager.RequerySuggested += value; }
            remove { if (_canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Construct a command with two methods, first for execute a command and second allow to execute the first method if return true
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute">the method that determines whether the command can execute in its current state.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute is null");
            _canExecute = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) => _canExecute == null ? true : _canExecute();

        /// <summary>
        ///  Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) =>  _execute();
    }

    /// <summary>
    /// Defines a relay command with Execute and Can Execute delegate, and cast parameter with type parameter T
    /// </summary>
    /// <typeparam name="T">Type of data used for Execute and CanExecute function</typeparam>
    public sealed class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// the method that determines whether the command can execute in its current state.
        /// </summary>
        readonly Action<T> _execute;

        /// <summary>
        /// the method that determines whether the command can execute in its current state.
        /// </summary>
        readonly Func<T,bool> _canExecute;

        /// <summary>
        /// <para>Represents the method that will handle an event that has no event data.</para>
        /// <para>And subscribe the method to CommandManager.RequerySuggested.</para>
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { if (_canExecute != null) CommandManager.RequerySuggested += value; }
            remove { if (_canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Construct a command with two methods, first for execute a command and second allow to execute the first method if return true
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute">the method that determines whether the command can execute in its current state.</param>
        public RelayCommand(Action<T> execute, Func<T,bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute is null");
            _canExecute = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) => _canExecute == null ? true : _canExecute((T) parameter);

        /// <summary>
        ///  Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter) => _execute((T) parameter);
    }
}
