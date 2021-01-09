using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Thomas.Apis.Core;
using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.Wpf.Core;

namespace Thomas.Apis.Presentation.Wpf.Controls
{
    [New("Real dynamic update of items")]
    public class AutoGrid : Grid
    {
        public AutoGrid()
        {
            //this.RowSizes = true;
            //this.ColumnSizes = true;
            //KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.);
            KeyboardNavigation.SetIsTabStop(this, false);
            this.LayoutUpdated += this.OnLayoutUpdated;
        }
        private bool m_initialized;

        private static IMetaData<AutoGrid> Metadata { get; } = Api.Global.MetaDataOf<AutoGrid>();


        public static readonly DependencyProperty ColumnSizesProperty =
            Metadata.DependencyProperty(cc => cc.ColumnSizes, OnColumnOrRowSizesChanged);

        private static void OnColumnOrRowSizesChanged(AutoGrid grid, string oldValue, string newValue)
        {
            //grid.InvalidateChildren();
        }

        public string ColumnSizes
        {
            get { return this.Get(x => x.ColumnSizes); }
            set { this.Set(x => x.ColumnSizes, value); }
        }

        public static readonly DependencyProperty RowSizesProperty =
            Metadata.DependencyProperty(cc => cc.RowSizes);

        public string RowSizes
        {
            get { return this.Get(x => x.RowSizes); }
            set { this.Set(x => x.RowSizes, value); }
        }

        public static readonly DependencyProperty HorizontalLayoutProperty =
            Metadata.DependencyProperty(cc => cc.HorizontalLayout, null, true);

        public bool HorizontalLayout
        {
            get { return this.Get(x => x.HorizontalLayout); }
            set { this.Set(x => x.HorizontalLayout, value); }

        }

        public static readonly DependencyProperty HorizontalAutoFillProperty =
          Metadata.DependencyProperty(cc => cc.HorizontalAutoFill);

        public bool HorizontalAutoFill
        {
            get { return this.Get(x => x.HorizontalAutoFill); }
            set { this.Set(x => x.HorizontalAutoFill, value); }
        }

        //public static readonly DependencyProperty SurroundElementsWithBorderProperty =
        // Metadata.DependencyProperty(cc => cc.SurroundElementsWithBorder);

        //public bool SurroundElementsWithBorder
        //{
        //    get { return this.Get(x => x.SurroundElementsWithBorder); }
        //    set { this.Set(x => x.SurroundElementsWithBorder, value); }
        //}


        public static readonly DependencyProperty DefaultSizeProperty =
            Metadata.DependencyProperty(cc => cc.DefaultSize, OnDefaultSizeChanged);

        private GridLength? DefaultGridLength { get; set; }

        private static void OnDefaultSizeChanged(AutoGrid grid, string oldValue, string newValue)
        {
            grid.DefaultGridLength = newValue == null ? default(GridLength?) : grid.CreateGridLength(newValue);
            //grid.InvalidateChildren();
        }

        public string DefaultSize
        {
            get { return this.Get(x => x.DefaultSize); }
            set { this.Set(x => x.DefaultSize, value); }
        }


        public static readonly DependencyProperty ItemsSourceProperty =
          Metadata.DependencyProperty(cc => cc.ItemsSource, OnItemsSourceChanged, options: FrameworkPropertyMetadataOptions.None);


        [Bindable(true)]
        public object ItemsSource
        {
            get { return this.Get(x => x.ItemsSource); }
            set { this.Set(x => x.ItemsSource, value); }

        }
        private static void OnItemsSourceChanged(AutoGrid grid, object? oldValue, object? newValue)
        {
            var sequence = newValue as IEnumerable<object> ?? newValue.To().Enumerable();
            grid.Children.Clear();
            sequence.BindTo(grid.Children.WrapAsTypedList(), (o,e)=>o ==e || o == (e as ContentPresenter)?.Content, x =>
            {
                var element = x as UIElement ?? CreateContentPresenter(x, grid);
                if (element != null)
                {
                    element.Visibility = Visibility.Collapsed;
                }
                return element;
            },e=>e!=null,null,grid.InvalidateChildren);
        }


        private static readonly ResourceTemplateSelector Selector = new ResourceTemplateSelector();
        private static UIElement? CreateContentPresenter(object? data,AutoGrid container)
        {
            var element = (FrameworkElement)Selector.SelectTemplate(data, container)?.LoadContent();// new ResourceContentPresenter{Content = data};

            if (element is ContentPresenter x)
            {
                x.Content = data;
            }
            else if (element is ContentControl y)
            {
                y.Content = data;
            }
            else if(element != null)
            {
                element.DataContext = data;
            
            }
      

            return element;
        }
        private void InvalidateChildren()
        {
            m_initialized = false;
            this.InvalidateVisual();
        }
        private void OnLayoutUpdated(object sender, EventArgs e)
        {

            if (m_initialized)
            {
                return;
            }
            m_initialized = true;

            this.CalculateRowsAndColumns();
        }

        private void CalculateRowsAndColumns()
        {
            var columnSizes = ColumnSizes?.Split(';') ?? new string[] { };
            var rowSizes = RowSizes?.Split(';') ?? new string[] { };


            var columnIndex = 0;
            var rowIndex = 0;

            var totalRowCount = 0;
            var totalColumnCount = 0;

            foreach (var c in this.Children.OfType<FrameworkElement>())
            {
                var customColumnIndex = GetColumn(c);
                var customRowIndex = GetRow(c);
                var effectiveColumnIndex = customColumnIndex == 0 ? columnIndex : customColumnIndex;
                var effectiveRowIndex = customRowIndex == 0 ? rowIndex : customRowIndex;


                SetColumn(c, effectiveColumnIndex);
                SetRow(c, effectiveRowIndex);


                totalColumnCount = Math.Max(totalColumnCount, columnIndex + 1);
                totalRowCount = Math.Max(totalRowCount, rowIndex + 1);

                if (HorizontalLayout)
                {
                    var span = GetColumnSpan(c);
                    columnIndex = columnIndex + Math.Max(1,span);
                    if (columnIndex < columnSizes.Length)
                        continue;

                    if (!HorizontalAutoFill || rowIndex < rowSizes.Length)
                    {
                        columnIndex = 0;
                        ++rowIndex;
                    }
                }
                else
                {
                    var span = GetRowSpan(c);
                    rowIndex = rowIndex + Math.Max(1, span);
                    if (rowIndex < rowSizes.Length)
                        continue;

                    if (HorizontalAutoFill || columnIndex < columnSizes.Length)
                    {
                        rowIndex = 0;
                        ++columnIndex;
                    }
                }
            }

            var minColumnCount = totalColumnCount;
            var minRowCount = totalRowCount;

            var indexToCountOffset = (DefaultGridLength?.IsAuto).GetValueOrDefault() ? 1 : 0;
            if (HorizontalAutoFill)
            {
                minColumnCount += indexToCountOffset;
            }
            else
            {
                minRowCount += indexToCountOffset;
            }

            for (var i = this.ColumnDefinitions.Count; i < minColumnCount; i++)
            {
                var gridLength = this.CreateGridLength(columnSizes.ElementAtOrDefault(i),
                    HorizontalAutoFill ? default(GridUnitType?) : GridUnitType.Star);
                this.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = gridLength
                });
            }

            for (var i = this.RowDefinitions.Count; i < minRowCount; i++)
            {
                var gridLength = this.CreateGridLength(rowSizes.ElementAtOrDefault(i),
                    HorizontalAutoFill ? GridUnitType.Star : default(GridUnitType?));
                this.RowDefinitions.Add(new RowDefinition
                {
                    Height = gridLength
                });
            }

            foreach (var c in this.Children.OfType<FrameworkElement>())
            {
                c.Visibility = Visibility.Visible;
            }
        }





        private GridLength CreateGridLength(string value, GridUnitType? defaultUnitType = null)
        {
            GridUnitType unitType;
            var unitValue = 1.0;

            switch (value)
            {
                default:
                    if (value == "*")
                    {
                        unitValue = 1.0;
                        unitType = GridUnitType.Star;
                    }
                    else if (value.EndsWith("*"))
                    {
                        unitValue = value.Substring(0, value.Length - 1).ToDouble();
                        unitType = GridUnitType.Star;
                    }
                    else
                    {
                        unitValue = value.ToDouble();
                        unitType = GridUnitType.Pixel;
                    }
                    break;
                case null:
                    if (DefaultGridLength == null)
                    {
                        unitType = defaultUnitType ?? GridUnitType.Star;
                    }
                    else
                    {
                        unitType = defaultUnitType ?? DefaultGridLength.Value.GridUnitType;
                        unitValue = DefaultGridLength.Value.Value;
                    }
                    break;
                case "Auto":
                    unitType = GridUnitType.Auto;
                    break;
            }
            //if (value == null)
            //{
            //    unitValue = 1.0;
            //}
            //else if (value.EndsWith("*"))
            //{
            //    unitValue = value.Substring(0, value.Length - 1).ToDouble();
            //}
            //else if (value == "Auto")
            //{
            //    unitValue = -1;
            //}
            //else
            //{
            //    unitValue = value.ToDouble();
            //}

            var gridLength = new GridLength(unitValue, unitType);
            return gridLength;
        }
    }

   

}