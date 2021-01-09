namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{
    public class View : BindAttribute
    {
        public View() : base(() => new View())
        {

        }
        public class Layout : BindAttribute
        {
            public Layout() : base(() => new Layout())
            {

            }
            public class List: BindAttribute
            {
                public List() : base(() => new List())
                {
                }
            }
            public class Grid : BindAttribute
            {
                public Grid() : base(() => new Grid())
                {

                }
                public class Cell : BindAttribute
                {
                    public Cell() : base(() => new Cell())
                    {

                    }

                    public string Margin
                    {
                        get => this.Get<string>();
                        set => this.Set(value);

                    }
                    public int RowSpan
                    {
                        get => this.Get(()=>1);
                        set => this.Set(value);

                    }

                    public int ColumnSpan
                    {
                        get => this.Get(() =>1);
                        set => this.Set(value);
                    }

                    public int ColumnIndex
                    {
                        get => this.Get(() => 0);
                        set => this.Set(value);
                    }

                    public int RowIndex
                    {
                        get => this.Get(() => 0);
                        set => this.Set(value);
                    }

                }

                public string ColumnSizes
                {
                    get => this.Get<string>();
                    set => this.Set(value);
                }

                public string RowSizes
                {
                    get => this.Get<string>();
                    set => this.Set(value);
                }

                public bool HorizontalLayout
                {
                    get => this.Get(()=> true);
                    set => this.Set(value);
                } 
                //public int Padding { get; set; }

                //public bool SurroundElementsWithBorder => this.Padding > 0;

                public bool DefaultSizeIsAuto
                {
                    get => this.Get(() => true);
                    set => this.Set(value);
                }

                public string DefaultSize => this.DefaultSizeIsAuto ? "Auto" : "*";

                public object HorizontalAutoFill
                {
                    get => this.Get(() => false);
                    set => this.Set(value);
                }

            }

            public class Text : BindAttribute
            {
                public Text() : base(() => new Text())
                {

                }
                public bool TextWrapping
                {
                    get => this.Get(() => false);
                    set => this.Set(value);
                }

                public TextAlignment Alignment
                {
                    get => this.Get(()=> TextAlignment.Left);
                    set => this.Set(value);
                }

                public bool IsReadOnly
                {
                    get => this.Get(() => false);
                    set => this.Set(value);
                }
            }

            public class Field : BindAttribute
            {
                public string Label
                {
                    get => this.Get<string>();
                    set => this.Set(value);
                }

                public Field(string? label = default!):base(()=>new Field())
                {
                    Label = label ?? " ";
                }
            }

            public class Frame : BindAttribute
            {

                public string Margin
                {
                    get => this.Get<string>();
                    set => this.Set(value);
                }
                public Frame() : base(() => new Frame())
                {

                }

            }
        }

     
    }
}