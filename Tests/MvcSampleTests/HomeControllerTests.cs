using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Moq;
using MvcSample.Core.Interfaces;
using MvcSample.Core.Model;
using MvcSample.ViewModels;
using MvcSample.Controllers;
using System.Collections.Generic;
using Xunit;

namespace MvcSampleTests
{
    public class HomeControllerTests
    {
        #region Get Action
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfBrainstormSessions()
        {
            // arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(x => x.ListAsync()).ReturnsAsync(GetTestSessions());
            var controller = new HomeController(mockRepo.Object);
            // act
            var result = await controller.Index();
            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<StormSessionViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        #endregion

        #region Post action
        [Fact]
        public async Task IndexPost_ReturnsBadRequestResult_WhenModelStateIsInvalid()
        {
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(x => x.ListAsync()).ReturnsAsync(GetTestSessions());
            var controller = new HomeController(mockRepo.Object);
            controller.ModelState.AddModelError("SessionName", "Required");
            var newSession = new StormSessionViewModel();
            //Act
            var result = await controller.Index(newSession);
            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task IndexPost_ReturnsARedirectAndAddsSession_WhenModelStateIsValid()
        {
            //Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(x=>x.AddAsync(It.IsAny<BrainstormSession>())).Returns(Task.CompletedTask).Verifiable();
            var controller = new HomeController(mockRepo.Object);
            var model = new StormSessionViewModel(){
                Name="Test Name"
            };
            //Act
            var result = await controller.Index(model);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index",redirectToActionResult.ActionName);
            mockRepo.Verify();

        }
        #endregion
        private List<BrainstormSession> GetTestSessions()
        {
            var sessions = new List<BrainstormSession>();
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2016, 7, 2),
                Id = 1,
                Name = "测试-讨论会01"
            });
            sessions.Add(new BrainstormSession()
            {
                DateCreated = new DateTime(2016, 7, 1),
                Id = 2,
                Name = "测试-讨论会02"
            });
            return sessions;
        }
    }
}