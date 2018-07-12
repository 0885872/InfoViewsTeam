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
        public void Index_ReturnsView_NotNull()
        {
            var returnedView = grp.Index() as ViewResult;
            Assert.NotNull(returnedView.ViewData["results"]);
            Assert.NotNull(returnedView.ViewData["UserInfoResults"]);
            Assert.NotNull(returnedView.ViewData["UserReservationsResults"]);
            //Input: nothing, just a method call. Output: 3x some value, != null

        }

    }
}
