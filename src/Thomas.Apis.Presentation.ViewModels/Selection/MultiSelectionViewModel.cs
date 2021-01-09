using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Thomas.Apis.Core.DotNet;

namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    public class MultiSelectionViewModel<TValue> : ViewModel<MultiSelectionViewModel<TValue>>, IMultiSelectionViewModel
    {
        public TValue[] InitialValues { get; }

        public MultiSelectionViewModel(params TValue[] initialValues)
        {
            InitialValues = initialValues;
        }

        public IList<TValue> SelectedValues => this.Get(f =>
        {
            var selectedValues = f.Collection<TValue>();
            this.Disposables += selectedValues.BindToCollectionChanged(x =>
            {
                if (x.EventArgs.Action != NotifyCollectionChangedAction.Move)
                {
                    var comparer = EqualityComparer<TValue>.Default;
                    foreach (var model in this.SelectionModels)
                    {
                        if ((x.EventArgs.NewItems ?? x.EventArgs.OldItems).Cast<TValue>().Any(v => comparer.Equals(v, model.Value)))
                        {
                            model.IsSelected = x.EventArgs.Action == NotifyCollectionChangedAction.Add ||
                                               x.EventArgs.Action == NotifyCollectionChangedAction.Reset;
                        }
                    }
                }

                this.RaisePropertyChanged(nameof(this.DisplayText), true);
            });

            return selectedValues;
        });

        public IEnumerable<SelectableItemViewModel<TValue>> SelectionModels => this.Get(f =>
        {
            var comparer = EqualityComparer<TValue>.Default;
            var collection = f.Collection<SelectableItemViewModel<TValue>>();

            this.Disposables += this.Values.BindTo(collection,
                (a, b) => comparer.Equals(a, b.Value), v => new SelectableItemViewModel<TValue>(this) { Value = v });

            var binding = default(IDisposable);
            void Bind()
            {
                // ReSharper disable once AccessToModifiedClosure
                binding?.Dispose();
                binding = collection.BindTo(CollectionBindMode.TwoWay, this.SelectedValues,
                    x => x.Value,
                    v => collection.Single(x => comparer.Equals(x.Value, v)),
                    svm => svm.IsSelected ?? false, v => false, true, (a, b) => comparer.Equals(a.Value, b));

                this.Disposables += binding;
            }

            collection.BindToCollectionItemsChanged((item, name) =>
            {
                if (name == nameof(ISelectableItemViewModel.IsSelected))
                {
                    if (item.IsSelected ?? false)
                    {
                        if (!this.SelectedValues.Contains(item.Value))
                        {
                            this.SelectedValues.Add(item.Value);
                        }    
                    }
                    else
                    {
                        if (this.SelectedValues.Contains(item.Value))
                        {
                            this.SelectedValues.Remove(item.Value);
                        }
                    }
                    this.FocusedModel = item;
                }
            });

            Bind();


            return collection;
        });

        public ISelectableItemViewModel FocusedModel
        {
            get => this.Get<ISelectableItemViewModel>();
            set => this.Set(value);
        }
        public IList<TValue> Values => this.Get(f => f.Collection(this.InitialValues));

        public override string DisplayText => this.SelectedValues.Select(x => x.ToString()).Aggregate();

        IEnumerable<ISelectableItemViewModel> IMultiSelectionViewModel.SelectionModels => this.SelectionModels;

        public bool IsSelectionVisible
        {
            get => this.Get(() => false);
            set => this.Set(value);
        }
    }
}