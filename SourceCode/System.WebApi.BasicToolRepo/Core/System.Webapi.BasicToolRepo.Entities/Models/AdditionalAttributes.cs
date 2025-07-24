using System.Diagnostics.CodeAnalysis;

namespace System.Webapi.BasicToolRepo.Entities.Models
{
    [ExcludeFromCodeCoverage]
    public class AdditionalAttributes
    {
        public string attributename { get; set; }
        public string attributevalue { get; set; }

        public AdditionalAttributes() { attributename = string.Empty;attributevalue = string.Empty; }

        public AdditionalAttributes(string attributename, string attributevalue)
        {
            this.attributename = attributename;
            this.attributevalue = attributevalue;
        }
    }
}
