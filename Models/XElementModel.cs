namespace XML_editor.Models
{
    public class XElementModel
    {
        public string Name { get; set; }
        public List<XElementModel> Children { get; set; } = new List<XElementModel>();
    }
}
 