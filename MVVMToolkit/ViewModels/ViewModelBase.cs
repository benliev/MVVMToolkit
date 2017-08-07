using MVVMToolkit.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MVVMToolkit.ViewModels
{
    /// <summary>
    /// View Model base implements notify property changed and notify data error info (works with data annotations on property)
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Contains value of all properties
        /// </summary>
        protected readonly IDictionary<string, object> properties = new Dictionary<string, object>();

        /// <summary>
        /// the method that will handle the PropertyChanged event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set new value if newValue is not same that current value an event is raised
        /// </summary>
        /// <param name="value">value to set to field</param>
        /// <param name="propertyName">name of the property called this method</param>
        protected void Set(object value, [CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            if (!properties.ContainsKey(propertyName) || properties[propertyName] != value)
            {
                properties[propertyName] = value;
                RaisePropertyChanged(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Get value 
        /// </summary>
        /// <typeparam name="T">Type of the newValue</typeparam>
        /// <param name="propertyName">name of the property called this method</param>
        protected dynamic Get([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            properties.TryGetValue(propertyName, out object value);
            return value;
        }

        /// <summary>
        /// Check if property with the name "propertyName" exists.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <exception cref="ArgumentException">If property name is null or empty</exception>
        /// <exception cref="ArgumentException">If don't found property with the property name</exception>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("The property name must be not null or empty !");
            }
            else if (GetType().GetProperty(propertyName) == null)
            {
                throw new ArgumentException($"The property with name {propertyName} doesn't exist !");
            }
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raise errors changed for the property with the name in param
        /// </summary>
        /// <param name="propertyName">The name of property where errors changed</param>
        public void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>Gets the error message for the property with the given name.</summary>
        /// <param name="columnName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property. The default is an empty string.</returns>
        protected IEnumerable<ValidationResult> ValidateProperty([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            object value = GetType().GetProperty(propertyName).GetValue(this);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(this) { MemberName = propertyName };
            Validator.TryValidateProperty(value, context, validationResults);
            return validationResults;
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or System.String.Empty, to retrieve entity-level errors.</param>
        /// <returns>The validation errors for the property or entity.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (string.IsNullOrEmpty(propertyName))
            {
                foreach (PropertyInfo property in GetType().GetProperties())
                    if (property.GetCustomAttributes(typeof(ValidationAttribute), true).Length > 0)
                        validationResults.AddRange(ValidateProperty(property.Name));
            }
            else
            {
                validationResults.AddRange(ValidateProperty(propertyName));
            }
            return validationResults;
        }

        /// <summary>
        /// Gets a value that indicates whether the entity has validation errors.
        /// </summary>
        /// <returns>true if the entity currently has validation errors; otherwise, false.</returns>
        public bool HasErrors => GetErrors(null).Cast<ValidationResult>().Count() > 0;

        /// <summary>
        /// <para>Create a command with a method to execute and a method to check if execute method can be invoked</para>
        /// <para>The methods dont use parameter</para>
        /// </summary>
        /// <param name="execute">method to execute</param>
        /// <param name="canExecute">method to check, can be set to null for no check</param>
        /// <returns>The command</returns>
        protected ICommand CreateCommand(Action execute, Func<bool> canExecute = null)
            => new RelayCommand(execute,canExecute);

        /// <summary>
        /// <para>Create a command with a method to execute and a method to check if execute method can be invoked</para>
        /// <para>The methods use parameter</para>
        /// </summary>
        /// <typeparam name="T">type of parameter methods</typeparam>
        /// <param name="execute">method to execute</param>
        /// <param name="canExecute">method to check, can be set to null for no check</param>
        /// <returns>The command</returns>
        protected ICommand CreateCommand<T>(Action<T> execute, Func<T, bool> canExecute = null)
            => new RelayCommand<T>(execute, canExecute);
    }
}
