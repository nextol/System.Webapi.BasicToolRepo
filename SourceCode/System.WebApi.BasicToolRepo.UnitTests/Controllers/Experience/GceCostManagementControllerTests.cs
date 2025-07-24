using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Webapi.BasicToolRepo;
using System.Webapi.BasicToolRepo.Contracts;
using System.Webapi.BasicToolRepo.Entities.Cost;
using System.Webapi.BasicToolRepo.Entities.Validation;
using System.Webapi.BasicToolRepo.Entities.ErrorModel;
using System.Webapi.BasicToolRepo.Factories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Webapi.BasicToolRepo.Contracts.InterFaces;
using System.Webapi.BasicToolRepo.Utilities;

namespace System.Webapi.BasicToolRepo.Tests.Controllers
{
    [TestFixture]
    public class GceCostManagementControllerTests
    {
        #region Private Properties
        #endregion
        #region SetUp
        [SetUp]
        public void Setup()
        {
            //This code block will run before each tests start
        }
        #endregion
        #region Test Cases
        [Test]
        public void MethodUnderTest_Scenario_ReturnExpectedValue()
        {
            Assert.Pass();
        }
        #endregion
        #region CommonMethods
        #endregion
        #region TearDown
        [TearDown]
        public void TearDown()
        {
            //This code block will run after each tests method end
        }
        #endregion
    }
}
