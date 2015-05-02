using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO;
using PcapngUtils.Extensions;
using NUnit.Framework;

namespace PcapngUtils.PcapNG.OptionTypes
{
    [ToString]
    [TestFixture]
    public sealed class SectionHeaderOption : AbstractOption
    {
        #region nUnitTest
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void SectionHeaderOption_ConvertToByte_Test(bool reorder)
        {
            SectionHeaderOption preOption = new SectionHeaderOption();
            SectionHeaderOption postOption;
            preOption.Comment = "Test Comment";
            preOption.Hardware = "x86 Personal Computer";
            preOption.OperatingSystem = "Windows 7";
            preOption.UserApplication = "PcapngUtils";
            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = SectionHeaderOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.Hardware, postOption.Hardware);
            Assert.AreEqual(preOption.OperatingSystem, postOption.OperatingSystem);
            Assert.AreEqual(preOption.UserApplication, postOption.UserApplication);
        }
        #endregion

        #region enum
        public enum SectionHeaderOptionCode : ushort
        {
            EndOfOptionsCode = 0,
            CommentCode = 1,
            HardwareCode = 2,
            OperatingSystemCode = 3,
            UserApplicationCode = 4
        };
        #endregion

        #region fields & properies
        /// <summary>
        /// A UTF-8 string containing a comment that is associated to the current block.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// An UTF-8 string containing the description of the hardware used to create this section.
        /// </summary>
        public string Hardware
        {
            get;
            set;
        }

        /// <summary>
        /// An UTF-8 string containing the name of the operating system used to create this section
        /// </summary>
        public string OperatingSystem
        {
            get;
            set;
        }

        /// <summary>
        /// An UTF-8 string containing the name of the application used to create this section.
        /// </summary>
        public string UserApplication
        {
            get;
            set;
        }
        #endregion

        #region ctor
        public SectionHeaderOption(string Comment = null, string Hardware = null, string OperatingSystem = null, string UserApplication = null)
        {
            this.Comment = Comment;
            this.Hardware = Hardware;
            this.OperatingSystem = OperatingSystem;
            this.UserApplication = UserApplication;
        }
        #endregion

        #region method
        public static SectionHeaderOption Parse(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");

            SectionHeaderOption option = new SectionHeaderOption();
            List<KeyValuePair<ushort, byte[]>> optionsList = EkstractOptions(binaryReader, reverseByteOrder, ActionOnException);
            if (optionsList.Any())
            {
                foreach (var item in optionsList)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case (ushort)SectionHeaderOptionCode.CommentCode:
                                option.Comment = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)SectionHeaderOptionCode.HardwareCode:
                                option.Hardware = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)SectionHeaderOptionCode.OperatingSystemCode:
                                option.OperatingSystem = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)SectionHeaderOptionCode.UserApplicationCode:
                                option.UserApplication = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)SectionHeaderOptionCode.EndOfOptionsCode:
                            default:
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        if (ActionOnException != null)
                            ActionOnException(exc);
                    }
                }   
            }
            return option;
        }

        public override byte[] ConvertToByte(bool reverseByteOrder, Action<Exception> ActionOnException)
        {

            List<byte> ret = new List<byte>();

            if (Comment != null)
            {
                byte[] comentValue = UTF8Encoding.UTF8.GetBytes(Comment);
                if (comentValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)SectionHeaderOptionCode.CommentCode, comentValue, reverseByteOrder, ActionOnException));
            }

            if (Hardware != null)
            {
                byte[] hardwareValue = UTF8Encoding.UTF8.GetBytes(Hardware);
                if (hardwareValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)SectionHeaderOptionCode.HardwareCode, hardwareValue, reverseByteOrder, ActionOnException));
            }

            if (OperatingSystem != null)
            {
                byte[] systemValue = UTF8Encoding.UTF8.GetBytes(OperatingSystem);
                if (systemValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)SectionHeaderOptionCode.OperatingSystemCode, systemValue, reverseByteOrder, ActionOnException));
            }

            if (UserApplication != null)
            {
                byte[] userAppValue = UTF8Encoding.UTF8.GetBytes(UserApplication);
                if (userAppValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)SectionHeaderOptionCode.UserApplicationCode, userAppValue, reverseByteOrder, ActionOnException));
            }


            ret.AddRange(ConvertOptionFieldToByte((ushort)SectionHeaderOptionCode.EndOfOptionsCode, new byte[0], reverseByteOrder, ActionOnException));
            return ret.ToArray();
        }
        #endregion
    }
}
