using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HPImageViewer
{
    public class ImageViewerCommand : ICommand
    {
        /// <summary>
        /// The <see cref="Action"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute"/> is used.
        /// </summary>
        private readonly Func<bool> canExecute;

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
        public ImageViewerCommand(Action execute)
        {
            //ArgumentNullException.ThrowIfNull(execute);

            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public ImageViewerCommand(Action execute, Func<bool> canExecute)
        {
            //ArgumentNullException.ThrowIfNull(execute);
            //ArgumentNullException.ThrowIfNull(canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke() != false;
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
