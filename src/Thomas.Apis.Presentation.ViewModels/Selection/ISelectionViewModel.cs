namespace Thomas.Apis.Presentation.ViewModels.Selection
{
    /// <summary>
    /// An abstraction of the generic selection view model.
    /// </summary>
    public interface ISelectionViewModel
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the selection is currently visible (e.g. for comboboxes)
        /// </summary>
        bool IsSelectionVisible { get; set; }
        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        int SelectedIndex { get; set; }
        /// <summary>
        /// Gets the display values.
        /// </summary>
        string[] DisplayValues { get;  }

        /// <summary>
        /// Gets the selected value of the selection view model.
        /// </summary>
        string SelectedText { get; set; }

        /// <summary>
        /// Gets or sets a value to determine whether the bound selection control is editable.
        /// </summary>
        bool IsEditable { get; set; }
    }
}