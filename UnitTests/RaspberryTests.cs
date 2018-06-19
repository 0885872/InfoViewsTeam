using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
  public class RaspberryTests
  {
    public Reserveer.Controllers.RaspberryController rb;

    public RaspberryTests()
    {
      Reserveer.Controllers.RaspberryController raspberry = new Reserveer.Controllers.RaspberryController();
      rb = raspberry;
    }
    //[Fact]
    //public void Index_ReturnsView_NotNull()
    //{
    //  var returnedView = rb.Index() as ViewResult;
    //  Assert.NotNull(returnedView.ViewData["results"]);
    //  Assert.NotNull(returnedView.ViewData["temp"]);

    //}
  }
}
