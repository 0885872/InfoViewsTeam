using System;
using Xunit;
using Reserveer;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    public class GroupAdminTests // Tests results from the GroupsAdminController
    {
    public Reserveer.Controllers.GroupsAdminController ad;

    public GroupAdminTests()
    {
      Reserveer.Controllers.GroupsAdminController admin = new Reserveer.Controllers.GroupsAdminController();
      ad = admin;
    }

    [Fact]
    public void Index_ReturnsView_NotNull()
    {
      var returnedView = ad.Index() as ViewResult;
      Assert.NotNull(returnedView.ViewData["results"]);
      Assert.NotNull(returnedView.ViewData["NumTimes"]);

    }

    [Fact]
    public void Addroom_ReturnsView_NotNull()
    {
      var result = ad.AddRoom();
      Assert.NotNull(result);

    }

    [Fact]
    public void Profile_ReturnsView_NotNull()
    {
      var returnedView = ad.Profile() as ViewResult;
      Assert.NotNull(returnedView.ViewData["results"]);
      Assert.NotNull(returnedView.ViewData["results2"]);
      Assert.NotNull(returnedView.ViewData["results3"]);
      Assert.NotNull(returnedView.ViewData["results4"]);
    }

    [Fact]
    public void Deactivatuser_ReturnsString_NotNull()
    {
    var result = ad.DeactivateUser("1","42");
    Assert.NotNull(result);

    }

    [Fact]
    public void DeleteReservation_ReturnsString_NotNull()
    {
    var result = ad.DeleteReservation("60" , "1");
    Assert.NotNull(result);

    } 

  }
}
