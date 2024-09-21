using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kasca.Common.Authrization;
using Kasca.Common.ComModels.Enums;

namespace Kasca.Common.Tests
{
    [TestClass]
    public class ElementIdTests
    {
        [TestMethod]
        public void ElementIdTest()
        {
            var idTye = ElementIdType.LoanGuid;
            idTye.InitialFromOldLoanOrLoanSheetId("Ins_");
            Assert.IsTrue(idTye==ElementIdType.InstalmentId);
        }
    }
}
