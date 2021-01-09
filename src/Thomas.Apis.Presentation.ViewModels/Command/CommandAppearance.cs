using Thomas.Apis.Core.New;

namespace Thomas.Apis.Presentation.ViewModels.Command
{
    /// <summary>
    /// Provides some meta data for commands.
    /// </summary>
    [New]
    public class CommandAppearance : NotifyObject
    {
        public Progress? Progress => this.Get(() => new Progress());

        public CommandAppearance(string? text = null, string? iconName = null, bool iconAfter = false)
        {
            Text = text;
            if (iconName != null)
            {
                var icon = new Icon { IconName = iconName };

                if (iconAfter)
                    this.IconAfterText = icon;
                else
                    this.IconBeforeText = icon;
            }
        }

        public Icon? IconBeforeText
        {
            get => this.Get<Icon>();
            set => this.Set(value);
        }
        public Icon? IconAfterText
        {
            get => this.Get<Icon>();
            set => this.Set(value);
        }

        /// <summary>
        /// Gets or sets the text of a command.
        /// </summary>
        public string? Text
        {
            get => this.Get<string>();
            set => this.Set(value);
        }


        /// <summary>
        /// An implicit cast from string to <see cref="CommandAppearance"/> for simple definition of use.
        /// </summary>
        /// <param name="text">The command text</param>
        public static implicit operator CommandAppearance(string text) => new CommandAppearance { Text = text };

        public static implicit operator CommandAppearance((Icon Icon, string Text) info)=> new CommandAppearance{ IconBeforeText = info.Icon, Text = info.Text};
        public static implicit operator CommandAppearance(( string Text, Icon Icon) info) => new CommandAppearance { IconAfterText = info.Icon, Text = info.Text };
    }
}
