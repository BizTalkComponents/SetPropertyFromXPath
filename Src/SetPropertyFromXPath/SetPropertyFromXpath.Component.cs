using System;
using System.Collections;
using System.Linq;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.SetPropertyFromXPath
{
    public partial class SetPropertyFromXPath
    {
        public string Name { get { return "SetPropertyFromXPath"; } }
        public string Version { get { return "1.0"; } }
        public string Description { get { return "Promotes a value from a specified XPath to a specified context property."; } }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("DF2A20CE-0000-0001-B4DC-0ADFF1E51111");
        }

        public void InitNew()
        {

        }

        public IEnumerator Validate(object projectSystem)
        {
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }

        public IntPtr Icon { get { return IntPtr.Zero; } }
    }
}
