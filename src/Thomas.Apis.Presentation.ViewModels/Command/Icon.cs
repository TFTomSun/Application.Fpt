using System;
using System.Drawing;
using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.ViewModels.Views;

namespace Thomas.Apis.Presentation.ViewModels.Command
{
    [New]
    public class Icon: ViewModel 
    {
        public string? IconName
        {
            get => this.Get(()=> nameof(PackIconKind.None));
            set => this.Set(value);
        }
        public Enum? IconValue
        {
            get => this.Get<Enum>();
            set => this.Set(value,x=>this.IconName = x.NewValue?.ToString());
        }

        public string? Color
        {
            get => this.Get<string>();
            set => this.Set(value);
        }

        public static implicit operator Icon(PackIconKind icon )
        {
            return new Icon()
            {
                IconValue = icon,
                Color = "Black"
            };
        }
        public static implicit operator Icon((PackIconKind Icon, String Color) info)
        {
            return new Icon()
            {
                IconValue = info.Icon,
                Color = info.Color,
            };
        }
    }
}