using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Thomas.Apis.Core.DotNet;

namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    /// <summary>
    /// A view model for string selection.
    /// </summary>
    public class SelectionViewModel<T> : ViewModel<SelectionViewModel<T>>, ISelectionViewModel
    {
        private Func<T, string> ConvertToText { get; }
        private IEnumerable<T> ValueQuery { get;  }
        private Func<string, T> ConvertToValue { get;  }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        public T DefaultValue
        {
            get { return this.Get(() => this.Values.FirstOrDefault()); }
            set { this.Set(value); }
        }

        /// <summary>
        /// Creates a new instance of the selection view model.
        /// </summary>
        /// <param name="valueQuery">The values that should be presented for selection</param>
        /// <param name="convertToText">The converter that converts the values into readable strings.</param>
        /// <param name="convertToValue"></param>
        /// <param name="defaultValue"></param>
        /// <param name="comparer"></param>
        public SelectionViewModel(IEnumerable<T> valueQuery = null,  Func<T,string> convertToText = null, Func<string,T> convertToValue = null, T defaultValue = default(T), Func<T,T,bool> comparer = null)
        {
            this.ConvertToText = convertToText ?? (v=>v?.ToString());
            this.ValueQuery = valueQuery ?? Enumerable.Empty<T>();
            this.ConvertToValue = convertToValue;
            if (defaultValue != null)
            {
                this.DefaultValue = defaultValue;
                this.SelectedText = this.ConvertToText(defaultValue);
            }

            this.Comparer = comparer ?? ((a,b)=>EqualityComparer<T>.Default.Equals(a,b));
        }

        private Func<T, T, bool> Comparer { get;  }

        /// <summary>
        /// Gets or sets a value to determine whether the bound selection control is editable.
        /// </summary>
        public bool IsEditable
        {
            get { return this.Get(() => false); }
            set { this.Set(value); }
        }


        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        public T SelectedValue
        {
            get { return this.Values.ElementAtOrDefault(this.SelectedIndex); }
            set { this.SelectedIndex = this.Values.IndexOf(value,false,this.Comparer); }
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return this.Get(() => this.Values.IndexOf(this.DefaultValue, false,this.Comparer)); }
            set
            {
                //var fixValue = value == -1 && this.Values.Length > 0;
                //var realValue = value == -1 && this.Values.Length > 0 ? 0 : value;

                this.Set(value,  x =>
                {
                    this.RaisePropertyChanged(nameof(this.SelectedValue), true);
                    this.RaisePropertyChanged(nameof(this.SelectedText), true);
                });

                //this.OnSelectedValueChanged?.Invoke(this.Values[value]);
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public IEnumerable<T> Values
        {
            get { return this.Get(() =>
            {
                if (this.HandleCollectionChanges(this.ValueQuery))
                {
                    return this.ValueQuery;
                }
                return this.ValueQuery.ToArray();
            }); }
            set { this.Set(value, x =>
            {
                this.ValuesContext.Dispose();

                this.HandleCollectionChanges(x.NewValue);
               
                //this.OnValuesChanged();
            }); }
        }

        private bool HandleCollectionChanges(IEnumerable<T> sequence)
        {
            var notifyCollection = sequence as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                this.ValuesContext += notifyCollection.BindToCollectionChanged<T>(args =>
                {
                    this.OnValuesChanged();
                });
                return true;
            }
            return false;

        }

        /// <summary>
        /// Can be overridden to perform derrived type specific operations on property changes.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(this.Values))
            {
                this.OnValuesChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnValuesChanged()
        {
            this.RaisePropertyChanged(nameof(this.SelectedValue), true);
            this.RaisePropertyChanged(nameof(this.SelectedIndex), true);
            this.RaisePropertyChanged(nameof(this.SelectedText), true);
            this.RaisePropertyChanged(nameof(this.DisplayValues), true);
        }

        private DisposableContainer ValuesContext { get; set; }= new DisposableContainer();

        /// <summary>
        /// Gets the display values that are bound to the UI.
        /// </summary>
        public string[] DisplayValues => this.Get(
            ()=> this.Values.Select(this.ConvertToText ?? (t => t.ToString())).ToArray());


        /// <summary>
        /// Gets the selected value of the selection view model.
        /// </summary>
        public string SelectedText
        {
            get {return this.Get(()=>this.ConvertToText(this.SelectedValue)); }
            set
            {
                this.Set(value,x =>
                {
                    if (this.ConvertToValue != null)
                    {
                        this.SelectedValue = this.ConvertToValue(x.NewValue);
                    }
                });
                
            }
        }

        public bool IsSelectionVisible 
        {
            get { return this.Get(() => false); }
            set { this.Set(value); }
        }


        public override string DisplayText => this.SelectedText;


    }
}