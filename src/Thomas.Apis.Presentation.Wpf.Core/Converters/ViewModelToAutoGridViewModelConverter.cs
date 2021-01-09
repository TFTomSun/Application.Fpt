using System;
using System.Globalization;
using Thomas.Apis.Presentation.ViewModels;
using Thomas.Apis.Presentation.ViewModels.Layouts;

namespace Thomas.Apis.Presentation.Wpf.Core.Converters
{
    public class ViewModelToAutoGridViewModelConverter : GenericValueConverter<IViewModel, AutoGridViewModel>
    {
        protected override AutoGridViewModel Convert(IViewModel value, object parameter, CultureInfo culture, Type targetType)
        {
            return value as AutoGridViewModel ?? new AutoGridViewModel(value);
        }

        protected override IViewModel ConvertBack(AutoGridViewModel value, object parameter, CultureInfo culture, Type targetType)
        {
            throw new NotImplementedException();
        }
    }
}