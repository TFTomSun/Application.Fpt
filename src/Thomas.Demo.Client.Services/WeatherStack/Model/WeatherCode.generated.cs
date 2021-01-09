using System;
using System.Collections.Generic;
using System.Text;

namespace Thomas.Demo.Client.Services.WeatherStack.Model
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class codes
    {

        private codesCondition[] conditionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("condition")]
        public codesCondition[] condition
        {
            get
            {
                return this.conditionField;
            }
            set
            {
                this.conditionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class codesCondition
    {

        private ushort codeField;

        private string descriptionField;

        private string day_iconField;

        private string night_iconField;

        /// <remarks/>
        public ushort code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string day_icon
        {
            get
            {
                return this.day_iconField;
            }
            set
            {
                this.day_iconField = value;
            }
        }

        /// <remarks/>
        public string night_icon
        {
            get
            {
                return this.night_iconField;
            }
            set
            {
                this.night_iconField = value;
            }
        }
    }


}
