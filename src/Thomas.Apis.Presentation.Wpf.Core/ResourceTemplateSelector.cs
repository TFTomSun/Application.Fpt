using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.Wpf.Core.Attached;

namespace Thomas.Apis.Presentation.Wpf.Core
{
    public class ResourceTemplateSelector : DataTemplateSelector
    {
        private static class ResourceTemplateSelectorState
        {
            public static readonly DataTemplate[] ApplicationDataTemplates =
                Application.Current == null ? new DataTemplate[] { } : Application.Current.Resources.ResourceValues<DataTemplate>();

            public static readonly DataTemplate FallbackTemplate =
                ApplicationDataTemplates.SingleOrDefault(dt => dt.DataTemplateKey?.ToString() == "ObjectTemplate");
        }

        public bool IsReadOnly { get; set; }
        public bool DisplayAsImage { get; set; }
        public bool Hierarchical { get; set; }
        public Type ExcludedDataTemplateType { get; set; }

        public ResourceTemplateSelector() : this(false, false)
        {
            
        }

        public ResourceTemplateSelector(bool isReadOnly, bool displayAsImage)
        {
            this.IsReadOnly = isReadOnly;
            this.DisplayAsImage = displayAsImage;
        }



        private struct DataTemplateKeyInfo
        {
            public Type DataType { get; set; }

            public bool IsReadOnly { get; set; }

            public bool DisplayAsImage { get; set; }

            public string ControlId { get; set; }

            public bool Hierarchical { get; set; }
            public Type ExcludedDataTemplateType { get; set; }

            public override string ToString()
            {
                var texts = new[]
                {
                    this.DataType?.ClassName(false),
                    this.IsReadOnly ? "R": "W",
                    this.DisplayAsImage ? "Image": null,
                    this.Hierarchical ?"Hierarchical":null,
                    this.ControlId,

                };
                return texts.Where(x=>x!=null).Aggregate(" ");
            }
        }

        private static readonly Dictionary<DataTemplateKeyInfo, DataTemplate> ApplicationDataTemplateMap = 
            new Dictionary<DataTemplateKeyInfo, DataTemplate>();

        private static readonly string EmptyGuid = Guid.Empty.ToString();

        [New("Let the last template win, that's the natural expected behavior")]
        public override DataTemplate SelectTemplate(object item, DependencyObject container = null)
        {
            var parent = container == null ? null: VisualTreeHelper.GetParent(container);
            var isReadOnly = (bool?) parent?.GetValue(ResourceTemplateSelectorExtensions.IsReadOnlyProperty);
            var useImage = (bool?) parent?.GetValue(ResourceTemplateSelectorExtensions.UseImageProperty);

            if (isReadOnly != null) this.IsReadOnly = isReadOnly.Value;
            if (useImage != null) this.DisplayAsImage = useImage.Value;

            if (item == null) return null;
            if (item is FrameworkElement) return null;

            var itemType = item.GetType();
           
            var customizableObject = container?.VisualParents().OfType<FrameworkElement>().Where(
                Attached.FrameworkElementExtensions.GetProvidesCustomDataTemplates).FirstOrDefault();

            var uid = customizableObject == null
                ? EmptyGuid
                : customizableObject.Uid.IsNullOrEmpty()
                    ? (customizableObject.Uid = Guid.NewGuid().ToString())
                    : customizableObject.Uid;

            var key = new DataTemplateKeyInfo
            {
                DataType = itemType,
                IsReadOnly = this.IsReadOnly,
                DisplayAsImage = this.DisplayAsImage,
                ControlId = uid,
                Hierarchical = this.Hierarchical,
                ExcludedDataTemplateType = this.ExcludedDataTemplateType,
            };
            if (!ApplicationDataTemplateMap.TryGetValue(key, out var dataTemplate))
            {
                //var frameworkElement = container as FrameworkElement;
                var allDataTemplates = ResourceTemplateSelectorState.ApplicationDataTemplates.Except(ResourceTemplateSelectorState.FallbackTemplate);
                if (customizableObject != null)
                {
                    var customDataTemplates = customizableObject.To().Enumerable().Concat(
                        customizableObject.VisualParents().OfType<FrameworkElement>()).Where(
                        Attached.FrameworkElementExtensions.GetProvidesCustomDataTemplates).SelectMany(
                        fe => fe.Resources.ResourceValues<DataTemplate>()).Where(dt=>dt.DataType != null);
                    allDataTemplates = customDataTemplates.Concat(allDataTemplates).ToArray();
                }
           
                var possibleTemplates = allDataTemplates.Where(
                    dt =>dt.Matches(item) && dt.IsReadOnly() == this.IsReadOnly && dt.IsHierarchical() == this.Hierarchical && this.ExcludedDataTemplateType != dt.FinalType()).OrderByDescending(x=>x.Priority()).ToArray();
                var exactTemplate = possibleTemplates.LastOrDefault(dt => dt.UseImage() == this.DisplayAsImage);
                if (exactTemplate == null && !this.IsReadOnly)
                {
                    //  display any editing template if there is no exact one rather than fallback
                    exactTemplate = possibleTemplates.LastOrDefault();
                }
                if (exactTemplate == null)
                {
                    exactTemplate = ResourceTemplateSelectorState.FallbackTemplate;
                }
                dataTemplate = exactTemplate;
               
               // dataTemplate = exactTemplate ?? ResourceTemplateSelectorState.FallbackTemplate;
                ApplicationDataTemplateMap.Add(key, dataTemplate);
            }
            return (DataTemplate)dataTemplate;
        }
    }
}