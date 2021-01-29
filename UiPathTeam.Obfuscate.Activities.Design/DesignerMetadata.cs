using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using UiPathTeam.Obfuscate.Activities.Design.Designers;
using UiPathTeam.Obfuscate.Activities.Design.Properties;

namespace UiPathTeam.Obfuscate.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(Obfuscate), categoryAttribute);
            builder.AddCustomAttributes(typeof(Obfuscate), new DesignerAttribute(typeof(ObfuscateDesigner)));
            builder.AddCustomAttributes(typeof(Obfuscate), new HelpKeywordAttribute(""));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
