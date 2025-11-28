using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public abstract class ObservableScriptableObject : ScriptableObject, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #nullable enable
        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler? PropertyChanging;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            this.PropertyChanging?.Invoke(this, e);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            if (true/*!Configuration.IsINotifyPropertyChangingDisabled*/) // TODO: expose bool to some global configuration is this should be invoked
            {
                OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            if (comparer.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            callback(newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            if (callback == null)
                throw new ArgumentNullException("callback");

            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            callback(newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null) where TModel : class
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (callback == null)
                throw new ArgumentNullException("callback");

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            callback(model, newValue);
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null) where TModel : class
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            if (model == null)
                throw new ArgumentNullException("model");

            if (callback == null)
                throw new ArgumentNullException("callback");

            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);
            callback(model, newValue);
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
