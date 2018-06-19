using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    public class ScheduleTests
    {
    public Reserveer.Controllers.ScheduleController sd;

    public ScheduleTests()
    {
      Reserveer.Controllers.ScheduleController schedule = new Reserveer.Controllers.ScheduleController();
      sd = schedule;
    }
        [Fact]
        public void Index_ReturnsViews_NotNull()
        {
      var returnedView = sd.Index() as ViewResult;
      Assert.NotNull(returnedView.ViewData["results"]);
      Assert.NotNull(returnedView.ViewData["NumTimes"]);
    }
  }
}
