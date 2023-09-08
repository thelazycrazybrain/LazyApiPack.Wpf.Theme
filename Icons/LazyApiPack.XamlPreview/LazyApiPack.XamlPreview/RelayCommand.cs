using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LazyApiPack.XamlPreview
{

	public class RelayCommand<TParameter> : ICommand
	{
		readonly Action<TParameter?> _execute;
		readonly Predicate<TParameter?>? _canExecute;

		public RelayCommand(Action<TParameter?> execute)
			: this(execute, null)
		{
		}


		public RelayCommand([NotNull] Action<TParameter?> execute, Predicate<TParameter?>? canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(TParameter? parameter)
		{
			return CanExecuteIntern(parameter);
		}

		private bool CanExecuteIntern(TParameter? parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		[DebuggerStepThrough]
		public bool CanExecute(object? parameter)
		{

			if (parameter is TParameter p)
			{
				return CanExecuteIntern(p);
			}
			else if (parameter == null)
			{
				return CanExecuteIntern(default(TParameter));
			}
			else
			{
				return false;
			}
		}

		public event EventHandler? CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		[DebuggerStepThrough]
		public void Execute(TParameter? parameter)
		{
			_execute(parameter);
		}

		[DebuggerStepThrough]
		public void Execute(object? parameter)
		{
			Execute((TParameter?)parameter);
		}
	}

	public class RelayCommand : ICommand
	{
		readonly Action<object?> _execute;
		readonly Predicate<object?>? _canExecute;

		public RelayCommand(Action<object?> execute)
			: this(execute, null)
		{
		}


		public RelayCommand([NotNull] Action<object?> execute, Predicate<object?>? canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}


		[DebuggerStepThrough]
		public bool CanExecute(object? parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public event EventHandler? CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		[DebuggerStepThrough]
		public void Execute(object? parameter)
		{
			_execute(parameter);
		}
	}
}
