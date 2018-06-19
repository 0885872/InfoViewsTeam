using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    public class GroupsTests
    {
        public Reserveer.Controllers.GroupsController grp;

        public GroupsTests()
        {
            Reserveer.Controllers.GroupsController group = new Reserveer.Controllers.GroupsController();
            grp = group;
        }
        [Fact]
        public void Test1()
        {
            var returnedView = grp.Index() as ViewResult;
            Assert.NotNull(returnedView.ViewData["results"]);
            Assert.NotNull(returnedView.ViewData["NumTimes"]);

        }

        [Fact]
        public void Test2()
        {
            var returnedView = grp.Rooms("TestNameUser") as ViewResult;
            Assert.NotNull(returnedView);
            Assert.NotNull(returnedView.ViewData["results"]);
            Assert.NotNull(returnedView.ViewData["Message"]);
            Assert.NotNull(returnedView.ViewData["NumTimes"]);

        }
    }
}
