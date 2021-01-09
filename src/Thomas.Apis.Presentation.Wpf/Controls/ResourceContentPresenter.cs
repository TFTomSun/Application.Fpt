using System.Windows.Controls;
using Thomas.Apis.Presentation.Wpf.Core;

namespace Thomas.Apis.Presentation.Wpf.Controls
{
    public class ResourceContentPresenter:ContentPresenter
    {
        private static ResourceTemplateSelector TemplateSelector = new ResourceTemplateSelector();
        public ResourceContentPresenter()
        {
            this.ContentTemplateSelector = TemplateSelector;
        }


    }
}