using System;
using BizTalkComponents.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;

namespace BizTalkComponents.PipelineComponents.SetPropertyFromXPath.Tests.UnitTests
{
    [TestClass]
    public class SetPropertyTests
    {
        [TestMethod]
        public void TestSetPropertyFromXpathPromote()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new SetPropertyFromXPath
            {
                PropertyPath = "http://tempuri.org#MyProp",
                XPath = "/root/test",
                PromoteProperty = true
            };

            pipeline.AddComponent(component, PipelineStage.Decode);

            var message = MessageHelper.Create("<root><test>TestValue</test></root>");

            Assert.IsNull(message.Context.Read("MyProp", "http://tempuri.org"));

            var output = pipeline.Execute(message);

            Assert.AreEqual(1, output.Count);

            Assert.IsTrue(output[0].Context.IsPromoted("MyProp", "http://tempuri.org"));
        }

        
        [TestMethod]
        public void TestSetPropertyFromXPathWrite()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new SetPropertyFromXPath
            {
                PropertyPath = "http://tempuri.org#MyProp",
                XPath = "/root/test",
                PromoteProperty = false
            };

            pipeline.AddComponent(component, PipelineStage.Decode);

            var message = MessageHelper.Create("<root><test>TestValue</test></root>");

            Assert.IsNull(message.Context.Read("MyProp", "http://tempuri.org"));

            var output = pipeline.Execute(message);

            Assert.AreEqual(1, output.Count);

            Assert.IsFalse(output[0].Context.IsPromoted("MyProp", "http://tempuri.org"));
        }

        
        [TestMethod]
        public void TestSetPropertyFromXPathPromotePropertyNotSet()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new SetPropertyFromXPath
            {
                PropertyPath = "http://tempuri.org#MyProp",
                XPath = "/root/test",
            };

            pipeline.AddComponent(component, PipelineStage.Decode);

            var message = MessageHelper.Create("<root><test>TestValue</test></root>");

            Assert.IsNull(message.Context.Read("MyProp", "http://tempuri.org"));

            var output = pipeline.Execute(message);

            Assert.AreEqual(1, output.Count);

            Assert.IsFalse(output[0].Context.IsPromoted("MyProp", "http://tempuri.org"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestNoMatchThrow()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new SetPropertyFromXPath
            {
                PropertyPath = "http://tempuri.org#MyProp",
                XPath = "/root/test/wrong",
                PromoteProperty = true,
                ThrowIfNoMatch = true
            };

            pipeline.AddComponent(component, PipelineStage.Decode);

            var message = MessageHelper.Create("<root><test>TestValue</test></root>");
            message.Context.Promote(new ContextProperty("http://tempuri.org#Source"), "Test");

            var output = pipeline.Execute(message);
        }

        [TestMethod]
        public void TestSetPropertyFromXpathAttributePromote()
        {
            var pipeline = PipelineFactory.CreateEmptyReceivePipeline();
            var component = new SetPropertyFromXPath
            {
                PropertyPath = "http://tempuri.org#MyProp",
                XPath = "/root/test/@testAttribute",
                PromoteProperty = true
            };

            pipeline.AddComponent(component, PipelineStage.Decode);

            var message = MessageHelper.Create("<root><test testAttribute='testAttributeValue'>TestValue</test></root>");

            Assert.IsNull(message.Context.Read("MyProp", "http://tempuri.org"));

            var output = pipeline.Execute(message);

            Assert.AreEqual(1, output.Count);
            Assert.AreNotEqual(output[0].Context.Read("MyProp", "http://tempuri.org"), string.Empty);
            Assert.IsTrue(output[0].Context.IsPromoted("MyProp", "http://tempuri.org"));
        }
    }
}
