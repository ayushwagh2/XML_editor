using System.Collections.Generic;
using System.Xml.Linq;

namespace XML_editor.Models
{
    public class XmlNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<XmlNode> Children { get; set; }
        public bool HasChildren => Children != null && Children.Count > 0;

        public XmlNode()
        {
            Attributes = new Dictionary<string, string>();
            Children = new List<XmlNode>();
        }

        public static XmlNode FromXElement(XElement element)
        {
            var node = new XmlNode
            {
                Name = element.Name.LocalName,
                Value = element.HasElements ? null : element.Value?.Trim()
            };

            foreach (var attribute in element.Attributes())
            {
                node.Attributes[attribute.Name.LocalName] = attribute.Value;
            }

            foreach (var child in element.Elements())
            {
                node.Children.Add(FromXElement(child));
            }

            return node;
        }

        public XElement ToXElement()
        {
            var element = new XElement(Name);

            if (!string.IsNullOrEmpty(Value))
            {
                element.Value = Value;
            }

            foreach (var attr in Attributes)
            {
                element.Add(new XAttribute(attr.Key, attr.Value));
            }

            foreach (var child in Children)
            {
                element.Add(child.ToXElement());
            }

            return element;
        }
    }
} 