using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.BizTalk.XPath;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.SetPropertyFromXPath
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [System.Runtime.InteropServices.Guid("D219F6DA-0000-0001-B0C3-24F764A22148")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public partial class SetPropertyFromXPath : IComponent, IBaseComponent, IPersistPropertyBag, IComponentUI
    {
        private const string PropertyPathPropertyName = "PropertyPath";
        private const string XPathPropertyName = "XPath";
        private const string PromoteProperytName = "Promote";
        private const string ThrowIfNoMatchPropertyName = "ThrowIfNoMatch";

        [DisplayName("Property Path")]
        [Description("The property path where the specified value will be promoted to, i.e. http://temupuri.org#MyProperty.")]
        [RegularExpression(@"^.*#.*$",
         ErrorMessage = "A property path should be formatted as namespace#property.")]
        [RequiredRuntime]
        public string PropertyPath { get; set; }

        [DisplayName("XPath")]
        [Description("The XPath to the value that should be promoted to the specified property.")]
        [RequiredRuntime]
        public string XPath { get; set; }

        [DisplayName("Promote Property")]
        [Description("Specifies whether the property should be promoted or just written to the context.")]
        [RequiredRuntime]
        public bool PromoteProperty { get; set; }

        [RequiredRuntime]
        [DisplayName("Throw if no match")]
        [Description("Specified whether an InvalidOperationException should be thrown if XPath does not exist.")]
        public bool ThrowIfNoMatch { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            String value = null;

            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            XmlTextReader xmlTextReader = new XmlTextReader(readOnlySeekableStream);
            XPathCollection xPathCollection = new XPathCollection();
            XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);
            xPathCollection.Add(XPath);

            while (xPathReader.ReadUntilMatch())
            {
                if (xPathReader.Match(0))
                {
                    if (xPathReader.NodeType == XmlNodeType.Attribute)
                    {
                        value = xPathReader.GetAttribute(xPathReader.Name);
                    }
                    else
                    {
                        value = xPathReader.ReadString();
                    }
                    
                    if (PromoteProperty)
                    {
                        pInMsg.Context.Promote(new ContextProperty(PropertyPath), value);
                    }
                    else
                    {
                        pInMsg.Context.Write(new ContextProperty(PropertyPath), value);
                    }

                    break;
                }
            }

            if (string.IsNullOrEmpty(value) && ThrowIfNoMatch)
            {
                throw new InvalidOperationException("The specified XPath did not exist or contained an empty value.");
            }

            readOnlySeekableStream.Position = 0;
            pContext.ResourceTracker.AddResource(readOnlySeekableStream);
            bodyPart.Data = readOnlySeekableStream;

            return pInMsg;
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            PropertyPath = PropertyBagHelper.ReadPropertyBag(propertyBag, PropertyPathPropertyName, PropertyPath);
            XPath = PropertyBagHelper.ReadPropertyBag(propertyBag, XPathPropertyName, XPath);
            PromoteProperty = PropertyBagHelper.ReadPropertyBag(propertyBag, PromoteProperytName, PromoteProperty);
            ThrowIfNoMatch = PropertyBagHelper.ReadPropertyBag(propertyBag, ThrowIfNoMatchPropertyName, ThrowIfNoMatch);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, PropertyPathPropertyName, PropertyPath);
            PropertyBagHelper.WritePropertyBag(propertyBag, XPathPropertyName, XPath);
            PropertyBagHelper.WritePropertyBag(propertyBag, PromoteProperytName, PromoteProperty);
            PropertyBagHelper.WritePropertyBag(propertyBag, ThrowIfNoMatchPropertyName, ThrowIfNoMatch);
        }
    }
}
